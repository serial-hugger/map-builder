using Newtonsoft.Json; 
using System.Collections.Generic; 
namespace MapBuilder.Shared.SerializationModels{ 

    public class Settings
    {
        [JsonProperty("generation_version")]
        public int GenerationVersion { get; set; }

        [JsonProperty("types")]
        public List<Type> Types { get; set; }

        [JsonProperty("retrieve")]
        public List<Retrieve> Retrieve { get; set; }
    }

}