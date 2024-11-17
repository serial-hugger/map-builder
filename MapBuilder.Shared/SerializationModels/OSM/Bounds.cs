using Newtonsoft.Json;

namespace MapBuilder.Shared.SerializationModels{ 

    public class Bounds
    {
        [JsonProperty("minlat")]
        public double MinLat { get; set; }
        [JsonProperty("minlon")]
        public double MinLng { get; set; }
        [JsonProperty("maxlat")]
        public double MaxLat { get; set; }
        [JsonProperty("maxlon")]
        public double MaxLng { get; set; }
    }

}