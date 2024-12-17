using Newtonsoft.Json;

namespace OpenNGS.SDK.Core.Network
{
    public class NetworkResponse
    {
        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public class NetworkResponse<T> : NetworkResponse
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
