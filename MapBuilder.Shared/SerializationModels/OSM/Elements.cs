using Newtonsoft.Json;

namespace MapBuilder.Shared.SerializationModels{ 

    public class Elements
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }
        [JsonProperty("nodes")]
        public List<long> Nodes { get; set; }
        [JsonProperty("geometry")]
        public List<Geometry> Geometry { get; set; }
        [JsonProperty("tags")]
        public Dictionary<string,string> Tags { get; set; }
    }

}