using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MapBuilder.Shared;

[Serializable]
public class Feature
{
    [Key]
    [Newtonsoft.Json.JsonIgnore]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("way")]
    public long WayId { get; set; }
    [JsonPropertyName("nodes")]
    public int TotalPoints { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
    [JsonPropertyName("closed")]
    public bool Closed { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonPropertyName("cell")]
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

    public void SetDataRetrieve(Dictionary<string,string>? tags, FeatureSettings? featureSettings)
    {
        FeatureSettings featureSettingsJson;
        string jsonFilepath;
        string jsonContent;
        if (featureSettings == null)
        {
            jsonFilepath = "Settings/featuresettings.json";
            jsonContent = System.IO.File.ReadAllText(jsonFilepath);
            featureSettingsJson = JsonConvert.DeserializeObject<FeatureSettings>(jsonContent);
        }
        else
        {
            featureSettingsJson = featureSettings;
        }
        
        string data = "";
        if (tags != null && tags.Any())
        {
            for (int tag = 0; tag < tags.Count(); tag++)
            {
                for (int retrieveSearch = 0; retrieveSearch < featureSettingsJson.Retrieve.Count; retrieveSearch++)
                {
                    try
                    {
                        string[] tagSplit = Regex.Unescape(tags.ElementAt(tag).ToString().Replace("\"", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty)).Split(',');
                        if (tagSplit[0].Trim() == featureSettingsJson.Retrieve?[retrieveSearch]?.Key.Trim())
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
                            dataString.Append(featureSettingsJson.Retrieve?[retrieveSearch]?.Label.ToString().Trim());
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
    public void SetProperties(List<Geometry> geometries, Dictionary<string,string>? tags, FeatureSettings? featureSettings)
    {
        FeatureSettings featureSettingsJson;
        string jsonFilepath;
        string jsonContent;
        if (featureSettings == null)
        {
            jsonFilepath = "Settings/featuresettings.json";
            jsonContent = System.IO.File.ReadAllText(jsonFilepath);
            featureSettingsJson = JsonConvert.DeserializeObject<FeatureSettings>(jsonContent);
        }
        else
        {
            featureSettingsJson = featureSettings;
        }
        int generationVersion = (int)featureSettingsJson.GenerationVersion;
        SetDataRetrieve(tags,featureSettingsJson);
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
                for (int typeSearch = 0; typeSearch < featureSettingsJson.Types.Count(); typeSearch++)
                {
                    string name = featureSettingsJson.Types[typeSearch].Name.ToString();
                    for (int tagSearch = 0; tagSearch < featureSettingsJson.Types[typeSearch].Tags.Count(); tagSearch++)
                    {
                        bool keySatisfied = false;
                        bool valueSatisfied = false;
                        if (featureSettingsJson.Types?[typeSearch]?.Tags?[tagSearch]?.Key == null
                            || tags.ElementAt(tag).Key.Contains(
                                featureSettingsJson.Types?[typeSearch]?.Tags?[tagSearch]?.Key?.ToString() ??
                                string.Empty))
                        {
                            keySatisfied = true;
                        }

                        if (featureSettingsJson.Types?[typeSearch]?.Tags?[tagSearch]?.Value == null
                            || tags.ElementAt(tag).Value.Contains(
                                featureSettingsJson.Types?[typeSearch]?.Tags?[tagSearch]?.Value ??
                                string.Empty))
                        {
                            valueSatisfied = true;
                        }
                        if (keySatisfied && valueSatisfied)
                        {
                            Type = name;
                        }
                    }
                }
            }
        }
    }
} 