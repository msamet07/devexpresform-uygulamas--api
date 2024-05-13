using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apiproject
{
    public class PdfResponse
    {

        [JsonProperty("data")]
        public string data { get; set; }

        [JsonProperty("succeed")]
        public bool succeed { get; set; }

        [JsonProperty("message")]
        public object message { get; set; }

        [JsonProperty("errorCode")]
        public object errorCode { get; set; }

        [JsonProperty("afterValue")]
        public int afterValue { get; set; }
    }
}
