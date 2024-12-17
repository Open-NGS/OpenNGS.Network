using Newtonsoft.Json;
using System.Net.Http;

namespace OpenNGS.SDK.Auth.Models.Requests
{
    public class BaseRequest
    {
        private string nonce;

        public string appId { get; set; }

        public long timestamp { get; set; }

        public string sign { get; set; }

        public BaseRequest(string appId, string sign, string nonce)
        {
            this.appId = appId;
            this.sign = sign;
            this.nonce = nonce;
        }

        public StringContent ToContent()
        {
            return new StringContent(JsonConvert.SerializeObject(this), System.Text.Encoding.UTF8, "application/json");
        }
    }
}
