using Newtonsoft.Json;

namespace MapBuilder.Shared.SerializationModels{ 

    public class Osm3s
    {
        [JsonProperty("timestamp_osm_base")]
        public DateTime TimestampOsmBase { get; set; }
        [JsonProperty("copyright")]
        public string Copyright { get; set; }
    }

}