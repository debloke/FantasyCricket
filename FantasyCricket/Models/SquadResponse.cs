using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasyCricket.Models
{
    
    public class SquadResponse
    {

        [JsonProperty("squad",Required = Required.Always)]
        public Team[] Teams { get; set; }

    }
}
