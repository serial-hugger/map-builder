using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MapBuilder.Shared;

[Serializable]
public class Way
{
    [Key]
    public int Id { get; set; }
    public Int64 WayId { get; set; }
    public int TotalNodes { get; set; }
    public string Type { get; set; }
    public bool Closed { get; set; }
    public bool Filled { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public int CellId { get; set; }
    public Way(Int64 wayId)
    {
        this.WayId  = wayId;
    }

    private Way(int cellId, Int64 wayId)
    {
        this.CellId = cellId;
        this.WayId = wayId;
    }

    public void SetProperties(JToken nodeIds, JToken? tags)
    {
        string jsonFilepath = "Settings/settings.json";
        string jsonContent = File.ReadAllText(jsonFilepath);
        JToken jsonToken = JsonConvert.DeserializeObject<JToken>(jsonContent);
        int generationVersion = (int)jsonToken["generation_version"];
        if (nodeIds[0].ToString() == nodeIds[nodeIds.Count() - 1].ToString())
        {
            TotalNodes = nodeIds.Count() - 1;
            Closed = true;
        }
        else
        {
            TotalNodes = nodeIds.Count();
            Closed = false;
        }

        if (tags != null && tags.Any())
        {
            Console.WriteLine($"way:{WayId} tags: {tags.Count()}");
            for (int tag = 0; tag < tags.Count(); tag++)
            {
                Console.WriteLine($"tag:{tag}");
                for (int typeSearch = 0; typeSearch < jsonToken["types"].Count(); typeSearch++)
                {
                    Console.WriteLine($"typeSearch:{typeSearch}");

                    string name = jsonToken["types"][typeSearch]["name"].ToString();
                    bool fillIfClosed = (bool)jsonToken["types"][typeSearch]["fill_if_closed"];

                    for (int tagSearch = 0; tagSearch < jsonToken["types"][typeSearch]["tags"].Count(); tagSearch++)
                    {
                        Console.WriteLine($"tagSearch:{tagSearch}");
                        try
                        {
                            bool keySatisfied = false;
                            bool valueSatisfied = false;
                            if (jsonToken["types"]?[typeSearch]?["tags"]?[tagSearch]?["key"] == null
                                || tags.ElementAt(tagSearch).ToString().Split(':')[0].Contains(
                                    jsonToken["types"]?[typeSearch]?["tags"]?[tagSearch]?["key"]?.ToString() ??
                                    string.Empty))
                            {
                                keySatisfied = true;
                            }

                            if (jsonToken["types"]?[typeSearch]?["tags"]?[tagSearch]?["value"] == null
                                || tags.ElementAt(tagSearch).ToString().Split(':')[1].Contains(
                                    jsonToken["types"]?[typeSearch]?["tags"]?[tagSearch]?["value"]?.ToString() ??
                                    string.Empty))
                            {
                                valueSatisfied = true;
                            }

                            if (keySatisfied && valueSatisfied)
                            {
                                Type = name;
                                if (Closed && fillIfClosed)
                                {
                                    Filled = true;
                                }
                                else
                                {
                                    Filled = false;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
    }
} 