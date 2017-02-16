using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace BivrostGateway
{
    class WebConnector
    {
        private WebClient m_client;
        WebConnector()
        {
            m_client = new WebClient();
            m_client.Headers.Add(HttpRequestHeader.Accept, "*/*");
            m_client.Headers[HttpRequestHeader.ContentType] = "application/json";

        }

        public string getLast()
        {
            WebClient webservice = new WebClient();
            webservice.Headers.Add(HttpRequestHeader.Accept, "*/*");
            webservice.Headers[HttpRequestHeader.ContentType] = "application/json";
            string URL = "http://localhost:8080";
            try
            {
                string result = webservice.DownloadString(URL + "/webapi/temperature/getLast");
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return "";
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ConnectFailure)
                    System.Threading.Thread.Sleep(1500);
                return "faild";
            }
        }
        public string insertOne()
        {
            string resultus = "";
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                resultus = client.UploadString("http://localhost:8080/webapi/temperature/insert", "POST", "{\"hardwareId\":8,\"sensorId\":8,\"temperature\":21.0}");
            }
            Console.WriteLine(resultus);
            return resultus;
        }

       /* http post method 
        *private static void post()
        {
            var baseAddress = "http://localhost:8080/webapi/temperature/insert";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            string parsedContent = "{\"hardwareId\":2,\"sensorId\":3,\"temperature\":21.0}";
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var resposta = http.GetResponse();

            var stream = resposta.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
        }*/
    }
}
