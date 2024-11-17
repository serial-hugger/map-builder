using Newtonsoft.Json;

namespace MapBuilder.Shared.SerializationModels{ 

    public class OSM
    {
        [JsonProperty("version")]
        public double Version { get; set; }
        [JsonProperty("generator")]
        public string Generator { get; set; }
        [JsonProperty("osm3s")]
        public Osm3s Osm3s { get; set; }
        [JsonProperty("elements")]
        public List<Element> Elements { get; set; }
    }

}