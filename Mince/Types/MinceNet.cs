
using System;
using System.Net;

namespace Mince.Types
{
    [StaticClass("net")]
    public class MinceNet : MinceObject
    {
        public MinceNet()
        {
            CreateMembers();
        }

        private WebClient client = new WebClient();

        [Exposed]
        public MinceBool ping(MinceString url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url.value.ToString());
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new MinceBool(response != null && response.StatusCode == HttpStatusCode.OK);
                response.Close();
                return result;
            }
            catch
            {
                return new MinceBool(false);
            }
        }

        [Exposed]
        public MinceString downloadString(MinceString url)
        {
            return new MinceString(client.DownloadString(url.value.ToString()));
        }

        [Exposed]
        public MinceNull downloadFile(MinceString url, MinceString path)
        {
            client.DownloadFile(url.value.ToString(), path.value.ToString());
            return new MinceNull();
        }
    }
}
