using Newtonsoft.Json; 
namespace MapBuilder.Shared.SerializationModels{ 

    public class Type
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }
    }

}