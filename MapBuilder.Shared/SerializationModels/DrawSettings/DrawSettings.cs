using Newtonsoft.Json; 
using System.Collections.Generic; 
namespace MapBuilder.Shared.SerializationModels{ 

    public class DrawSettings
    {
        [JsonProperty("modes")]
        public List<Mode> Modes { get; set; }
    }

}