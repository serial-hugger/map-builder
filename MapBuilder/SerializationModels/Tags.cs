using Newtonsoft.Json; 
namespace MapBuilder{ 

    public class Tags
    {
        public string highway { get; set; }
        public string name { get; set; }
        public string oneway { get; set; }
        public string service { get; set; }
        public string bridge { get; set; }
        public string layer { get; set; }
        public string width { get; set; }
        public string tracktype { get; set; }
        public string leisure { get; set; }
        public string sport { get; set; }
        public string wikidata { get; set; }
        public string mapillary { get; set; }
        public string surface { get; set; }

        [JsonProperty("survey:date")]
        public string surveydate { get; set; }
        public string foot { get; set; }
        public string sac_scale { get; set; }
        public string trail_visibility { get; set; }
        public string wheelchair { get; set; }
        public string landuse { get; set; }
        public string meadow { get; set; }
        public string lit { get; set; }
        public string hgv { get; set; }
        public string lanes { get; set; }
        public string maxspeed { get; set; }
        public string @ref { get; set; }

        [JsonProperty("sidewalk:right")]
        public string sidewalkright { get; set; }
        public string trolley_wire { get; set; }

        [JsonProperty("source:maxspeed")]
        public string sourcemaxspeed { get; set; }

        [JsonProperty("zone:traffic")]
        public string zonetraffic { get; set; }
        public string handrail { get; set; }
        public string source { get; set; }
        public string cables { get; set; }
        public string circuits { get; set; }
        public string frequency { get; set; }
        public string @operator { get; set; }

        [JsonProperty("operator:wikidata")]
        public string operatorwikidata { get; set; }

        [JsonProperty("operator:wikipedia")]
        public string operatorwikipedia { get; set; }
        public string power { get; set; }
        public string voltage { get; set; }
        public string wires { get; set; }
        public string incline { get; set; }
        public string bicycle { get; set; }
        public string cycleway { get; set; }

        [JsonProperty("addr:city")]
        public string addrcity { get; set; }

        [JsonProperty("addr:country")]
        public string addrcountry { get; set; }

        [JsonProperty("addr:housenumber")]
        public string addrhousenumber { get; set; }

        [JsonProperty("addr:postcode")]
        public string addrpostcode { get; set; }

        [JsonProperty("addr:street")]
        public string addrstreet { get; set; }
        public string building { get; set; }
        
    }

}