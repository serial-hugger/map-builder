using System.Collections.Generic; 
namespace MapBuilder{ 

    public class Root
    {
        public double version { get; set; }
        public string generator { get; set; }
        public Osm3s osm3s { get; set; }
        public List<Element> elements { get; set; }
    }

}