using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BivrostGateway
{
    class JsonPost

    {
        private string urlToPost = "";

        public JsonPost(string urlToPost)
        {
            this.urlToPost = urlToPost;
        }

        public bool postData(Dictionary<string, object> dictData)
        {
            WebClient webClient = new WebClient();
            byte[] resByte;
            string resString;
            byte[] reqString;

            try
            {
                webClient.Headers["content-type"] = "application/json";
                reqString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(dictData, Formatting.Indented));
                resByte = webClient.UploadData(this.urlToPost, "post", reqString);
                resString = Encoding.Default.GetString(resByte);
                Console.WriteLine(resString);
                webClient.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}
