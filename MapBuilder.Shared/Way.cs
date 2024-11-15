using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
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

    public string? RetrievedData { get; set; }
    public Way(Int64 wayId)
    {
        this.WayId  = wayId;
    }

    private Way(int cellId, Int64 wayId)
    {
        this.CellId = cellId;
        this.WayId = wayId;
    }

    public void SetDataRetrieve(JToken? tags, JToken jsonToken)
    {
        string data = "";
        if (tags != null && tags.Any())
        {
            for (int tag = 0; tag < tags.Count(); tag++)
            {
                for (int retrieveSearch = 0; retrieveSearch < jsonToken["retrieve"].Count(); retrieveSearch++)
                {
                    try
                    {
                        string[] tagSplit = Regex.Unescape(tags.ElementAt(tag).ToString().Replace("\"", string.Empty)).Split(':');
                        if (tagSplit[0].Trim() == jsonToken["retrieve"]?[retrieveSearch]?["key"].ToString().Trim())
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
                            dataString.Append(jsonToken["retrieve"]?[retrieveSearch]?["label"].ToString().Trim());
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
            RetrievedData = data.ToString();
            Console.WriteLine(data.ToString());
        }
    }
    public void SetProperties(JToken nodeIds, JToken? tags)
    {
        string jsonFilepath = "Settings/settings.json";
        string jsonContent = File.ReadAllText(jsonFilepath);
        JToken jsonToken = JsonConvert.DeserializeObject<JToken>(jsonContent);
        int generationVersion = (int)jsonToken["generation_version"];
        SetDataRetrieve(tags,jsonToken);
        if (nodeIds[0].ToString() == nodeIds[nodeIds.Count()-1].ToString())
        {
            Closed = true;
        }
        else
        {
            Closed = false;
        }
        TotalNodes = nodeIds.Count();

        if (tags != null && tags.Any())
        {
            for (int tag = 0; tag < tags.Count(); tag++)
            {
                for (int typeSearch = 0; typeSearch < jsonToken["types"].Count(); typeSearch++)
                {
                    string name = jsonToken["types"][typeSearch]["name"].ToString();
                    bool fillIfClosed = (bool)jsonToken["types"][typeSearch]["fill_if_closed"];

                    for (int tagSearch = 0; tagSearch < jsonToken["types"][typeSearch]["tags"].Count(); tagSearch++)
                    {
                        try
                        {

                            bool keySatisfied = false;
                            bool valueSatisfied = false;
                            if (jsonToken["types"]?[typeSearch]?["tags"]?[tagSearch]?["key"] == null
                                || tags.ElementAt(tag).ToString().Split(':')[0].Contains(
                                    jsonToken["types"]?[typeSearch]?["tags"]?[tagSearch]?["key"]?.ToString() ??
                                    string.Empty))
                            {
                                keySatisfied = true;
                            }

                            if (jsonToken["types"]?[typeSearch]?["tags"]?[tagSearch]?["value"] == null
                                || tags.ElementAt(tag).ToString().Split(':')[1].Contains(
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
                        catch(Exception ex)
                        {
                            Console.WriteLine($"Failed tag search... way:{WayId} tag:{tag} typeSearch:{typeSearch} tagSearch:{tagSearch}");
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
    }
} 