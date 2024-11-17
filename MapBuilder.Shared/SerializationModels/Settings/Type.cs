using Newtonsoft.Json; 
using System.Collections.Generic; 
namespace MapBuilder.Shared.SerializationModels{ 

    public class Type
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fill_if_closed")]
        public bool FillIfClosed { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }
    }

}