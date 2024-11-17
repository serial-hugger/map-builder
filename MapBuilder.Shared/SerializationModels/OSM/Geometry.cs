using Newtonsoft.Json;

namespace MapBuilder.Shared.SerializationModels{ 

    public class Geometry
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("lon")]
        public double Lon { get; set; }
    }

}