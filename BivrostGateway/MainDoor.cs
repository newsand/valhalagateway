using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BivrostGateway
{
    class MainDoor
    {
        static bool _continue;
        static SerialPort _serialPort;
        public static string palavra;

        public static void Main()
        {
            string name;
            string message;




            //WebConnector a = new WebConnector();
            //TemperatureRegister c = a.getLast();
            //TemperatureRegister v_temp = new TemperatureRegister(12, 12, DateTimeOffset.UtcNow, 25.4);
            //a.postData(v_temp.toDictionary());
            SerialConector a = new SerialConector();
            a.SerialConnect();
            a.startWorking();
            Console.ReadKey();
            a.stopListening();
            Console.WriteLine("bye");
           
        }
    }
}