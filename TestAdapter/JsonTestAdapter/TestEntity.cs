using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTestAdapter
{
    using Newtonsoft.Json;

    public class Test
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Outcome { get; set; }
    }
}
