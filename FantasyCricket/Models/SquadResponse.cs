using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FantasyCricket.Models
{
    
    public class SquadResponse
    {

        [JsonProperty("squad")]
        public Team[] Teams { get; set; }

    }
}
