using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ValhallaGateway
{
    class SerialConector
    {
        #region Memberes
        private static int K_CONETION_RETRIES = 5;
        private int m_conectionTries;
        private bool m_continue;
        
        private SerialPort m_serialPort;
        private Thread m_readingThread;
        private Thread m_writingThread;
        private Thread m_postingThread;
        #endregion

        #region Create
        public SerialConector(String p_portName,int p_boudRate, Parity p_parityConfig, int p_dataBits, StopBits p_stopBits, int p_readTimeout, int p_writeTimeout, Handshake p_handshake)
        {
            //m_serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            m_serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            m_serialPort.Handshake = p_handshake;
            m_serialPort.WriteTimeout = p_writeTimeout;
            m_serialPort.ReadTimeout = p_readTimeout;
            m_readingThread = new Thread(Read);
            m_readingThread = new Thread(Read);
            m_readingThread = new Thread(Read);
        }
        //Destructor
        ~SerialConector()
        {
            Dispose(false);
        }
        #endregion

        #region Threading
        public void StartReading()
        {
            m_continue = true;
            m_readingThread.Start();
        }
        public void Startposting()
        {
            m_continue = true;
            m_postingThread.Start();
        }
        public void StartWriting()
        {
            m_continue = true;
            m_writingThread.Start();
        }
        private void Read()
        {
            while (m_continue)
            {
                try
                {
                    string message = m_serialPort.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }
        private void Post()
        {
            
        }
        private void write()
        {

        }
        public void StopReading()
        {
            m_continue = false;
            //Wait for thread to finish
            m_readingThread.Join(/*thread timeout + 50ms*/450);
            //Kill frozen thread
            if (m_readingThread.IsAlive)
                m_readingThread.Abort();
        }
        #endregion

        #region ManualSerialConfig
        // Display Port values and prompt user to enter a port.
        public static string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }
        // Display BaudRate values and prompt user to enter a value.
        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
            baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }

        // Display PortParity values and prompt user to enter a value.
        public static Parity SetPortParity(Parity defaultPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
            parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity, true);
        }
        // Display DataBits values and prompt user to enter a value.
        public static int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits;

            Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
            dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            return int.Parse(dataBits.ToUpperInvariant());
        }

        // Display StopBits values and prompt user to enter a value.
        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available StopBits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
             "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }
        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
            handshake = Console.ReadLine();

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
        }
        #endregion

        #region Serial Conection
        public void SerialConnect()
        {
            m_serialPort.Open();
        }

        public void SerialDisconnect()
        {
            m_serialPort.Close();
        }
        public void SerialReconect()
        {
            if (m_conectionTries <= K_CONETION_RETRIES)
            {
                if (m_serialPort.IsOpen)
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
                m_conectionTries += 1;
            }
        }

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

    }
}
