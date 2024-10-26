using System.Collections.Generic; 
namespace MapBuilder{ 

    public class Element
    {
        public string type { get; set; }
        public int id { get; set; }
        public Bounds bounds { get; set; }
        public List<Int64> nodes { get; set; }
        public List<Geometry> geometry { get; set; }
        public Tags tags { get; set; }
    }

}