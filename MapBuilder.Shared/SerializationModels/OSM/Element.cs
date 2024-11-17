using Newtonsoft.Json;

namespace MapBuilder.Shared.SerializationModels{ 

    public class Element
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("ref")]
        public long? Ref { get; set; }
        [JsonProperty("role")]
        public string? Role { get; set; }
        [JsonProperty("lat")]
        public double? Lat { get; set; }
        [JsonProperty("lon")]
        public double? Lon { get; set; }
        [JsonProperty("bounds")]
        public Bounds? Bounds { get; set; }
        [JsonProperty("nodes")]
        public List<long>? Nodes { get; set; }
        [JsonProperty("geometry")]
        public List<Geometry>? Geometry { get; set; }
        [JsonProperty("tags")]
        public Dictionary<string,string>? Tags { get; set; }
        [JsonProperty("members")]
        public List<Element>? Members { get; set; }
    }

}