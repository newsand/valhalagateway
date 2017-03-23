using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace BivrostGateway
{
    class WebConnector
    {
        private WebClient m_client;
        //        string m_baseURL = "http://localhost:8080";
        string m_baseURL = "https://midgardserver.herokuapp.com"; 
        public WebConnector()
        {
            m_client = new WebClient();
            m_client.Headers.Add(HttpRequestHeader.Accept, "*/*");
            m_client.Headers[HttpRequestHeader.ContentType] = "application/json";

        }

        public TemperatureRegister getLast()
        {
            try
            {
                string result = m_client.DownloadString(m_baseURL + "/webapi/temperature/getLast");
                if (result != null)
                {
                    TemperatureRegister b = JsonConvert.DeserializeObject<TemperatureRegister>(result);
                    return b;
                }
                else
                {
                    return null;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ConnectFailure)
                    System.Threading.Thread.Sleep(1500);
                return null;
            }
        }

        public bool postData(Dictionary<string, object> dictData)
        {
            WebClient webClient = new WebClient();
            byte[] v_encodedResponse;
            string v_result;
            byte[] v_requestString;

            try
            {
                webClient.Headers["content-type"] = "application/json";
                v_requestString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(dictData, Formatting.Indented));
                v_encodedResponse = webClient.UploadData(m_baseURL + "/webapi/temperature/insert", "post", v_requestString);
                v_result = Encoding.Default.GetString(v_encodedResponse);
                Console.WriteLine(v_result);
                webClient.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
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
