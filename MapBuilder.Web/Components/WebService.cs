using System.Text.Json.Serialization;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json;

namespace MapBuilder.Web.Components;

public class WebService
{
    public FeatureSettings FeatureSettings { get; set; }
    public DrawSettings DrawSettings { get; set; }
    public string FeatureSettingsEditing { get; set; } = "{\n  \"generation_version\": 0,\n  \"types\": [\n    {\n      \"name\": \"water\",\n      \"tags\": [\n        {\n          \"key\": \"natural\",\n          \"value\": \"water\"\n        },\n        {\n          \"key\": \"water\",\n          \"value\": null\n        },\n        {\n          \"key\": \"waterway\",\n          \"value\": null\n        }\n      ]\n    },\n    {\n      \"name\": \"road\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"primary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"secondary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"tertiary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"residential\"\n        }\n      ]\n    },\n    {\n      \"name\": \"path\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"path\"\n        }\n      ]\n    },\n    {\n      \"name\": \"building\",\n      \"tags\": [\n        {\n          \"key\": \"building\",\n          \"value\": null\n        }\n      ]\n    }\n  ],\n  \"retrieve\": [\n    {\n      \"label\": \"name\",\n      \"key\": \"name\"\n    }\n  ]\n}";
    public string DrawSettingsEditing { get; set; } = "{\n  \"modes\": [\n    {\n      \"order\": 0,\n      \"type_name\": \"water\",\n      \"stroke_color\": \"#00bcff\",\n      \"fill_color\": \"#00bcff\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    },\n    {\n      \"order\": 1,\n      \"type_name\": \"road\",\n      \"stroke_color\": \"#000000\",\n      \"fill_color\": \"#000000\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 3\n    },\n    {\n      \"order\": 1,\n      \"type_name\": \"path\",\n      \"stroke_color\": \"#d0a655\",\n      \"fill_color\": \"#d0a655\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 2\n    },\n    {\n      \"order\": 2,\n      \"type_name\": \"building\",\n      \"stroke_color\": \"#8e8e8e\",\n      \"fill_color\": \"#8e8e8e\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 1\n    }\n  ]\n}";
    
    public void LoadDefaults()
    {
        string jsonFilepath;
        string jsonContent;
        if (FeatureSettings == null)
        {
            jsonFilepath = "Settings/featuresettings.json";
            jsonContent = System.IO.File.ReadAllText(jsonFilepath);
            FeatureSettings = JsonConvert.DeserializeObject<FeatureSettings>(jsonContent);
            FeatureSettingsEditing = jsonContent;
        }

        if (DrawSettings == null)
        {
            jsonFilepath = "Settings/drawsettings.json";
            jsonContent = System.IO.File.ReadAllText(jsonFilepath);
            DrawSettings = JsonConvert.DeserializeObject<DrawSettings>(jsonContent);
        }
    }
    public bool SaveFeatureSettings()
    {
        FeatureSettings? featureSettingsTemp;
        try
        {
            featureSettingsTemp =
                JsonConvert.DeserializeObject<FeatureSettings>(FeatureSettingsEditing);
            if (featureSettingsTemp != null)
            {
                featureSettingsTemp.GenerationVersion++;
                FeatureSettings = featureSettingsTemp;
                FeatureSettingsEditing = JsonConvert.SerializeObject(FeatureSettings, Formatting.Indented);
                return true;
            }
        }
        catch
        {
            return false;
        }
        return false;
    }
    public bool SaveDrawSettings()
    {
        DrawSettings? drawSettingsTemp;
        try
        {
            drawSettingsTemp =
                JsonConvert.DeserializeObject<DrawSettings>(DrawSettingsEditing);
            if (drawSettingsTemp != null)
            {
                DrawSettings = drawSettingsTemp;
                DrawSettingsEditing = JsonConvert.SerializeObject(DrawSettings, Formatting.Indented);
                return true;
            }
        }
        catch
        {
            return false;
        }
        return false;
    }
}