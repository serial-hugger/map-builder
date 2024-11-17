using Newtonsoft.Json; 
namespace MapBuilder.Shared.SerializationModels{ 

    public class Tag
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

}