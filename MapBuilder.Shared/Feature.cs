using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MapBuilder.Shared;

[Serializable]
public class Feature
{
    [Key]
    public int Id { get; set; }
    public long WayId { get; set; }
    public int TotalPoints { get; set; }
    public string Type { get; set; }
    public bool Closed { get; set; }
    public bool Filled { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public int CellId { get; set; }

    public string? RetrievedData { get; set; }
    public Feature(Int64 wayId)
    {
        this.WayId  = wayId;
    }

    private Feature(int cellId, Int64 wayId)
    {
        this.CellId = cellId;
        this.WayId = wayId;
    }

    public void SetDataRetrieve(Dictionary<string,string>? tags, Settings settingsJson)
    {
        string data = "";
        if (tags != null && tags.Any())
        {
            for (int tag = 0; tag < tags.Count(); tag++)
            {
                for (int retrieveSearch = 0; retrieveSearch < settingsJson.Retrieve.Count; retrieveSearch++)
                {
                    try
                    {
                        string[] tagSplit = Regex.Unescape(tags.ElementAt(tag).ToString().Replace("\"", string.Empty)).Split(':');
                        if (tagSplit[0].Trim() == settingsJson.Retrieve?[retrieveSearch]?.Key.Trim())
                        {
                            StringBuilder dataString = new StringBuilder();
                            if (RetrievedData==null)
                            {
                                RetrievedData = "";
                            }

                            if (dataString.ToString()!="")
                            {
                                dataString.Append(';');
                            }
                            dataString.Append(settingsJson.Retrieve?[retrieveSearch]?.Label.ToString().Trim());
                            dataString.Append(':');
                            dataString.Append(tagSplit[1].Trim());
                            data += dataString.ToString();
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        if (data != null && data != "")
        {
            RetrievedData = data;
        }
    }
    public void SetProperties(List<Geometry> geometries, Dictionary<string,string>? tags)
    {
        string jsonFilepath = "Settings/featuresettings.json";
        string jsonContent = File.ReadAllText(jsonFilepath);
        Settings settingsJson = JsonConvert.DeserializeObject<Settings>(jsonContent);
        int generationVersion = (int)settingsJson.GenerationVersion;
        SetDataRetrieve(tags,settingsJson);
        if (geometries[0].Lat == geometries[^1].Lat && geometries[0].Lon == geometries[^1].Lon)
        {
            Closed = true;
        }
        else
        {
            Closed = false;
        }
        TotalPoints = geometries.Count;

        if (tags != null && tags.Any())
        {
            for (int tag = 0; tag < tags.Count(); tag++)
            {
                for (int typeSearch = 0; typeSearch < settingsJson.Types.Count(); typeSearch++)
                {
                    string name = settingsJson.Types[typeSearch].Name.ToString();
                    bool fillIfClosed = (bool)settingsJson.Types[typeSearch].FillIfClosed;

                    for (int tagSearch = 0; tagSearch < settingsJson.Types[typeSearch].Tags.Count(); tagSearch++)
                    {
                        bool keySatisfied = false;
                        bool valueSatisfied = false;
                        if (settingsJson.Types?[typeSearch]?.Tags?[tagSearch]?.Key == null
                            || tags.ElementAt(tag).Key.Contains(
                                settingsJson.Types?[typeSearch]?.Tags?[tagSearch]?.Key?.ToString() ??
                                string.Empty))
                        {
                            keySatisfied = true;
                        }

                        if (settingsJson.Types?[typeSearch]?.Tags?[tagSearch]?.Value == null
                            || tags.ElementAt(tag).Value.Contains(
                                settingsJson.Types?[typeSearch]?.Tags?[tagSearch]?.Value ??
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
                }
            }
        }
    }
} 