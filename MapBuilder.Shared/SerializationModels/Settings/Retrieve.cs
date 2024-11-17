using Newtonsoft.Json; 
namespace MapBuilder.Shared.SerializationModels{ 

    public class Retrieve
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }

}