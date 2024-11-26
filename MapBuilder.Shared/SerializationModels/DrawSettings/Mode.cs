using Newtonsoft.Json; 
namespace MapBuilder.Shared.SerializationModels{ 

    public class Mode
    {
        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("type_name")]
        public string TypeName { get; set; }

        [JsonProperty("stroke_color")]
        public string StrokeColor { get; set; }

        [JsonProperty("fill_color")]
        public string FillColor { get; set; }

        [JsonProperty("fill_if_closed")]
        public bool FillIfClosed { get; set; }

        [JsonProperty("stroke_thickness")]
        public int StrokeThickness { get; set; }
    }

}