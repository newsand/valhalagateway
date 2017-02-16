using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace BivrostGateway
{
    class WebConnector
    {
        public int getData()
        {
            //Object Serialization
            TemperatureRegister a = new TemperatureRegister(2, 3, DateTimeOffset.Now, 25.1);
 
             //string toPost = "{\"hardwareId\":2,\"registerTime\":\"2017-02-16T00:36:14.211Z\",\"sensorId\":3,\"temperature\":21.0}";
            string toPost = "{\"hardwareId\":2,\"sensorId\":3,\"temperature\":21.0}";
            string URL = "http://localhost:8080";
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(a);
            //Sending to webservice
            System.Diagnostics.Stopwatch myWatch = new System.Diagnostics.Stopwatch();
            myWatch.Start();
            WebClient webservice = new WebClient();
            webservice.Headers.Add(HttpRequestHeader.Accept, "*/*");
            try
            {
                //string response = webservice.UploadString(URL + "/webapi/temperature/insert", output);
                string response = webservice.UploadString("http://localhost:8080/webapi/temperature/insert", output);

                //string response = webservice.UploadString(URL + "/webapi/temperature/getLast");
                string result = webservice.DownloadString(URL + "/webapi/temperature/getLast");
                myWatch.Stop();
                if (result != null)
                {
                   // global.Logger.log("CA", "WS RESPONSE: " + response + " in " + myWatch.ElapsedMilliseconds + " ms");
                    return 0;
                }
                else
                {
            //        global.Logger.log("CN", "WS Err: Empty Message");
                }
            }
            catch (WebException ex)
            {
               // global.Logger.log("CN", "WS Err status: " + ex.Status);

                //Server offline sleep for 1.5s
                if (ex.Status == WebExceptionStatus.ConnectFailure)
                    System.Threading.Thread.Sleep(1500);
            }

            return -1; //Error
        }
    }
}
