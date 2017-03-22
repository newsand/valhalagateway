using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BivrostGateway
{
    class SerialConector
    {
        //port atributes
        private SerialPort serialPort;
        private int baudRate;
        private String portName;



        static string serialBuffer = "";
        static string expectedEcho = null;
        static object expectedEchoLock = new object();
        static ManualResetEvent expectedEchoReceived = new ManualResetEvent(false);


        //listening thread attributes
        public Thread listeningThread;
        public Thread sendThread;
        private Queue<byte> receiveBuffer;
        private Queue<TemperatureRegister> m_registereBuffer;
        private Queue<Tuple<DateTime, byte[]>> dataPacketBuffer;
        private Queue<byte[]> sendBuffer;
        private bool working;

        private int maxUnsucessfullConnections = 3;
        private int unsucessfullConnections;

        private const int RO_TIME = 30;

        #region Serial Connect/Disconnect
        public SerialConector(String serialParam = null)
        {
            this.portName = "COM3";

            this.listeningThread = new Thread(this.listenThreadFunction);
            this.listeningThread.Name = "beringListener";
            this.receiveBuffer = new Queue<byte>();
            this.m_registereBuffer=new Queue< TemperatureRegister > ();
            this.dataPacketBuffer = new Queue<Tuple<DateTime, byte[]>>();

            this.sendThread = new Thread(this.sendThreadFunction);
            this.sendThread.Name = "beringSender";
            this.sendBuffer = new Queue<byte[]>();

            this.baudRate = 9600;
            this.serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            this.serialPort.Handshake = Handshake.None;
            this.serialPort.WriteTimeout = 200;
            this.serialPort.ReadTimeout = 50;
        }

        public void SerialConnect()
        {
            try
            {
                serialPort.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("", "Cannot open serialPort err: " + e.Message);
                Environment.Exit(0);
            }
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                if (serialPort.Handshake == Handshake.None)
                {
                    serialPort.DtrEnable = false;
                    serialPort.RtsEnable = false;
                }
            }
        }

        public void SerialDisconnect()
        {
            serialPort.Close();
        }
        #endregion

        #region Thread functions (start, stop, reconect, destroy)
        public void startWorking()
        {
            working = true;
            sendThread.Start();
            listeningThread.Start();
        }


        public void stopListening()
        {
            working = false;
            //Wait for thread to finish
            listeningThread.Join(/*thread timeout + 50ms*/450);
            //Kill frozen thread
            if (listeningThread.IsAlive)
                listeningThread.Abort();
        }

        void Reconnect()
        {
            if (unsucessfullConnections <= maxUnsucessfullConnections)
            {
                if (serialPort.IsOpen)
                {
                    try
                    {
                        SerialDisconnect();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("", "Cannot disconnect serial. Error: " + ex.Message);
                    }
                }

                try
                {
                    SerialConnect();
                }
                catch (Exception ex)
                {
                   Console.WriteLine("", "Cannot reconnect serial. Error: " + ex.Message);
                }
                unsucessfullConnections += 1;
            }
        }

        //Destructor
        ~SerialConector()
        {
            Dispose(false);
        }

        //Disposable
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.SerialDisconnect();

            //Evita problemas com o garbage collector
            if (disposing)
                GC.SuppressFinalize(this);
        }
        #endregion

        #region Listening Thread function (do the hard work)

        private void listenThreadFunction()
        {
            byte currentByte;
            string v_currentLine;
            string[] v_splitedString;
            TemperatureRegister v_currentRegister;
            char[] delimiters = new char[] {';',' ',':' };
            while (working)
            {
                try
                {
                    v_currentLine = serialPort.ReadLine();
                    Console.WriteLine("CURRENT LINE: " + v_currentLine);
                    v_splitedString = v_currentLine.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    if (v_splitedString.Length <= 3)
                        continue;
                    else {
                        v_currentRegister = new TemperatureRegister(int.Parse(v_splitedString[1]),int.Parse(v_splitedString[3]),DateTimeOffset.Now,double.Parse(v_splitedString[5]));
                        this.m_registereBuffer.Enqueue(v_currentRegister);
                    }
                    currentByte = (byte)serialPort.ReadByte();
                    Console.WriteLine("CURRENT BYTE" + currentByte);
                    receiveBuffer.Enqueue(currentByte);
                }
                catch (Exception ex)
                {
                    //The specified port is not open. 
                    if (ex is InvalidOperationException)
                    {
                        Console.WriteLine("", "Serial exception: " + ex.Message);
                        Reconnect();
                    }
                    //No byte was read.
                    else if (ex is TimeoutException)
                    {
                        unsucessfullConnections = 0;
                    }
                }
            }
        }

        #endregion

        #region Sending thread function
        private void sendThreadFunction()
        {
            byte[] pktResponse;
            while (working)
            {
                if (sendBuffer.Count > 0)
                {
                    lock (sendBuffer)
                    {
                        pktResponse = sendBuffer.Dequeue();
                    }
                    serialPort.Write(pktResponse, 0, pktResponse.Length);
                    Thread.Sleep(RO_TIME);
                }
                Thread.Sleep(22);
            }

        }

        #endregion

        // Public interface
        #region Get packet from listener output buffer
        public int getPacketCount()
        {
            int packetCount = 0;
            lock (dataPacketBuffer)
                packetCount = dataPacketBuffer.Count;

            return packetCount;
        }

        public Tuple<DateTime, byte[]> DequeuePacket()
        {
            //Critical Session;
            lock (dataPacketBuffer)
            {
                if (getPacketCount() > 0)
                {
                    if (dataPacketBuffer.Count > 0)
                    {
                        return (dataPacketBuffer.Dequeue());
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    // No data to process
                    return null;
                }
            }
        }
        #endregion
        #region GET and SET
        public String getPortName()
        {
            return this.portName;
        }
        public void sendResetSignal()
        {
            serialPort.RtsEnable = true;
        }
        public void clearResetSignal()
        {
            serialPort.RtsEnable = false;
        }
        #endregion
    }
}

