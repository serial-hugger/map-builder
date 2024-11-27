using System.Text.Json.Serialization;
using MapBuilder.Data;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json;

namespace MapBuilder.Web.Components;

public class WebService
{
    public int GenerationVersion = 0;
    public FeatureSettings FeatureSettings { get; set; }
    public DrawSettings DrawSettings { get; set; }
    public string FeatureSettingsEditing { get; set; } = "{\n  \"generation_version\": 0,\n  \"types\": [\n    {\n      \"name\": \"water\",\n      \"tags\": [\n        {\n          \"key\": \"natural\",\n          \"value\": \"water\"\n        },\n        {\n          \"key\": \"water\",\n          \"value\": null\n        },\n        {\n          \"key\": \"waterway\",\n          \"value\": null\n        }\n      ]\n    },\n    {\n      \"name\": \"road\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"primary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"secondary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"tertiary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"residential\"\n        }\n      ]\n    },\n    {\n      \"name\": \"path\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"path\"\n        }\n      ]\n    },\n    {\n      \"name\": \"building\",\n      \"tags\": [\n        {\n          \"key\": \"building\",\n          \"value\": null\n        }\n      ]\n    },\n    {\n      \"name\": \"wood\",\n      \"tags\": [\n        {\n          \"key\": \"natural\",\n          \"value\": \"wood\"\n        }\n      ]\n    },\n    {\n      \"name\": \"park\",\n      \"tags\": [\n        {\n          \"key\": \"leisure\",\n          \"value\": \"park\"\n        }\n      ]\n    },\n    {\n      \"name\": \"railway\",\n      \"tags\": [\n        {\n          \"key\": \"railway\",\n          \"value\": null\n        }\n      ]\n    }\n  ],\n  \"retrieve\": [\n    {\n      \"label\": \"name\",\n      \"key\": \"name\"\n    }\n  ]\n}";
    public string DrawSettingsEditing { get; set; } = "{\n  \"modes\": [\n    {\n      \"order\": -1,\n      \"type_name\": \"base\",\n      \"stroke_color\": \"#217517\",\n      \"fill_color\": \"#217517\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    },\n    {\n      \"order\": 0,\n      \"type_name\": \"park\",\n      \"stroke_color\": \"#c8f769\",\n      \"fill_color\": \"#c8f769\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    },\n    {\n      \"order\": 1,\n      \"type_name\": \"water\",\n      \"stroke_color\": \"#00bcff\",\n      \"fill_color\": \"#00bcff\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    },\n    {\n      \"order\": 2,\n      \"type_name\": \"wood\",\n      \"stroke_color\": \"#0f6401\",\n      \"fill_color\": \"#0f6401\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    },\n    {\n      \"order\": 4,\n      \"type_name\": \"road\",\n      \"stroke_color\": \"#000000\",\n      \"fill_color\": \"#000000\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 3\n    },\n    {\n      \"order\": 3,\n      \"type_name\": \"path\",\n      \"stroke_color\": \"#d0a655\",\n      \"fill_color\": \"#d0a655\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 2\n    },\n    {\n      \"order\": 5,\n      \"type_name\": \"railway\",\n      \"stroke_color\": \"#ff6b00\",\n      \"fill_color\": \"#ff6b00\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 4\n    },\n    {\n      \"order\": 6,\n      \"type_name\": \"building\",\n      \"stroke_color\": \"#8e8e8e\",\n      \"fill_color\": \"#8e8e8e\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 1\n    }\n  ]\n}";

    protected void OnInitialized()
    {
        FeatureSettings = JsonConvert.DeserializeObject<FeatureSettings>(FeatureSettingsEditing);
        DrawSettings = JsonConvert.DeserializeObject<DrawSettings>(DrawSettingsEditing);
    }
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
                using (var cell = new CellRepository())
                {
                    int version = cell.GetHighestGenerationVersion().Result;
                    featureSettingsTemp.GenerationVersion = version + 1;
                    FeatureSettings = featureSettingsTemp;
                    FeatureSettingsEditing = JsonConvert.SerializeObject(FeatureSettings, Formatting.Indented);
                    return true;
                }
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
    public void LoadDefaultFeature()
    {
        FeatureSettingsEditing = "{\n  \"generation_version\": 0,\n  \"types\": [" +
                                 "\n    {\n      \"name\": \"water\",\n      \"tags\": [\n        {\n          \"key\": \"natural\",\n          \"value\": \"water\"\n        },\n        {\n          \"key\": \"water\",\n          \"value\": null\n        },\n        {\n          \"key\": \"waterway\",\n          \"value\": null\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"road\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"primary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"secondary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"tertiary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"residential\"\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"path\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"path\"\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"building\",\n      \"tags\": [\n        {\n          \"key\": \"building\",\n          \"value\": null\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"wood\",\n      \"tags\": [\n        {\n          \"key\": \"natural\",\n          \"value\": \"wood\"\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"park\",\n      \"tags\": [\n        {\n          \"key\": \"leisure\",\n          \"value\": \"park\"\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"railway\",\n      \"tags\": [\n        {\n          \"key\": \"railway\",\n          \"value\": null\n        }\n      ]\n    }" +
                                 "\n  ]," +
                                 "\n  \"retrieve\": [" +
                                 "\n    {\n      \"label\": \"name\",\n      \"key\": \"name\"\n    }" +
                                 "\n  ]\n}";
    }
    public void LoadMinimalFeature()
    {
        FeatureSettingsEditing = "{\n  \"generation_version\": 0,\n  \"types\": [" +
                                 "\n    {\n      \"name\": \"water\",\n      \"tags\": [\n        {\n          \"key\": \"natural\",\n          \"value\": \"water\"\n        },\n        {\n          \"key\": \"water\",\n          \"value\": null\n        },\n        {\n          \"key\": \"waterway\",\n          \"value\": null\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"road\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"primary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"secondary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"tertiary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"residential\"\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"building\",\n      \"tags\": [\n        {\n          \"key\": \"building\",\n          \"value\": null\n        }" +
                                 "\n      ]\n    }\n  ]," +
                                 "\n  \"retrieve\": [" +
                                 "\n    {\n      \"label\": \"name\",\n      \"key\": \"name\"\n    }" +
                                 "\n  ]\n}";
    }
    public void LoadTransitFeature()
    {
        FeatureSettingsEditing = "{\n  \"generation_version\": 0,\n  \"types\": [" +
                                 "\n    {\n      \"name\": \"road\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"primary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"secondary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"tertiary\"\n        },\n        {\n          \"key\": \"highway\",\n          \"value\": \"residential\"\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"path\",\n      \"tags\": [\n        {\n          \"key\": \"highway\",\n          \"value\": \"path\"\n        }\n      ]\n    }," +
                                 "\n    {\n      \"name\": \"railway\",\n      \"tags\": [\n        {\n          \"key\": \"railway\",\n          \"value\": null\n        }" +
                                 "\n      ]\n    }\n  ]," +
                                 "\n  \"retrieve\": [" +
                                 "\n    {\n      \"label\": \"name\",\n      \"key\": \"name\"\n    }" +
                                 "\n  ]\n}";
    }

    public void LoadDefaultDraw()
    {
        DrawSettingsEditing = "{\n  \"modes\": [" +
                              "\n    {\n      \"order\": -1,\n      \"type_name\": \"base\",\n      \"stroke_color\": \"#217517\",\n      \"fill_color\": \"#217517\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 0,\n      \"type_name\": \"park\",\n      \"stroke_color\": \"#c8f769\",\n      \"fill_color\": \"#c8f769\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 1,\n      \"type_name\": \"water\",\n      \"stroke_color\": \"#00bcff\",\n      \"fill_color\": \"#00bcff\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 2,\n      \"type_name\": \"wood\",\n      \"stroke_color\": \"#0f6401\",\n      \"fill_color\": \"#0f6401\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 4,\n      \"type_name\": \"road\",\n      \"stroke_color\": \"#000000\",\n      \"fill_color\": \"#000000\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 3\n    }," +
                              "\n    {\n      \"order\": 3,\n      \"type_name\": \"path\",\n      \"stroke_color\": \"#d0a655\",\n      \"fill_color\": \"#d0a655\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 2\n    }," +
                              "\n    {\n      \"order\": 5,\n      \"type_name\": \"railway\",\n      \"stroke_color\": \"#ff6b00\",\n      \"fill_color\": \"#ff6b00\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 6,\n      \"type_name\": \"building\",\n      \"stroke_color\": \"#8e8e8e\",\n      \"fill_color\": \"#8e8e8e\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 1\n    }" +
                              "\n  ]\n}";
    }

    public void LoadTreasureMapDraw()
    {
        DrawSettingsEditing = "{\n  \"modes\": [" +
                              "\n    {\n      \"order\": -1,\n      \"type_name\": \"base\",\n      \"stroke_color\": \"#eab676\",\n      \"fill_color\": \"#eab676\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 0,\n      \"type_name\": \"park\",\n      \"stroke_color\": \"#f5d480\",\n      \"fill_color\": \"#f5d480\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 1,\n      \"type_name\": \"water\",\n      \"stroke_color\": \"#00bcff\",\n      \"fill_color\": \"#00bcff\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 2,\n      \"type_name\": \"wood\",\n      \"stroke_color\": \"#a1aca4\",\n      \"fill_color\": \"#a1aca4\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 4,\n      \"type_name\": \"road\",\n      \"stroke_color\": \"#7b4610\",\n      \"fill_color\": \"#7b4610\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 3\n    }," +
                              "\n    {\n      \"order\": 3,\n      \"type_name\": \"path\",\n      \"stroke_color\": \"#a76a59\",\n      \"fill_color\": \"#a76a59\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 2\n    }," +
                              "\n    {\n      \"order\": 5,\n      \"type_name\": \"railway\",\n      \"stroke_color\": \"#d8a195\",\n      \"fill_color\": \"#d8a195\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 6,\n      \"type_name\": \"building\",\n      \"stroke_color\": \"#e27038\",\n      \"fill_color\": \"#e27038\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 1\n    }" +
                              "\n  ]\n}";
    } 
    public void LoadOpenStreetMapDraw()
    {
        DrawSettingsEditing = "{\n  \"modes\": [" +
                              "\n    {\n      \"order\": -1,\n      \"type_name\": \"base\",\n      \"stroke_color\": \"#f3efe8\",\n      \"fill_color\": \"#f3efe8\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 0,\n      \"type_name\": \"park\",\n      \"stroke_color\": \"#c8fbcd\",\n      \"fill_color\": \"#c8fbcd\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 1,\n      \"type_name\": \"water\",\n      \"stroke_color\": \"#aad3df\",\n      \"fill_color\": \"#aad3df\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 2,\n      \"type_name\": \"wood\",\n      \"stroke_color\": \"#acd09e\",\n      \"fill_color\": \"#acd09e\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 4,\n      \"type_name\": \"road\",\n      \"stroke_color\": \"#f9f8f8\",\n      \"fill_color\": \"#f9f8f8\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 3\n    }," +
                              "\n    {\n      \"order\": 3,\n      \"type_name\": \"path\",\n      \"stroke_color\": \"#e7ccb6\",\n      \"fill_color\": \"#e7ccb6\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 2\n    }," +
                              "\n    {\n      \"order\": 5,\n      \"type_name\": \"railway\",\n      \"stroke_color\": \"#a19f9f\",\n      \"fill_color\": \"#a19f9f\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 6,\n      \"type_name\": \"building\",\n      \"stroke_color\": \"#cdc1b7\",\n      \"fill_color\": \"#cdc1b7\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 1\n    }" +
                              "\n  ]\n}";
    }
    public void LoadGoogleMapDraw()
    {
        DrawSettingsEditing = "{\n  \"modes\": [" +
                              "\n    {\n      \"order\": -1,\n      \"type_name\": \"base\",\n      \"stroke_color\": \"#f8f7f7\",\n      \"fill_color\": \"#f8f7f7\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 0,\n      \"type_name\": \"park\",\n      \"stroke_color\": \"#d3f9e3\",\n      \"fill_color\": \"#d3f9e3\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 1,\n      \"type_name\": \"water\",\n      \"stroke_color\": \"#90dbef\",\n      \"fill_color\": \"#90dbef\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 2,\n      \"type_name\": \"wood\",\n      \"stroke_color\": \"#c3f0d4\",\n      \"fill_color\": \"#c3f0d4\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 4,\n      \"type_name\": \"road\",\n      \"stroke_color\": \"#8ba5c1\",\n      \"fill_color\": \"#8ba5c1\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 3\n    }," +
                              "\n    {\n      \"order\": 3,\n      \"type_name\": \"path\",\n      \"stroke_color\": \"#d3dde5\",\n      \"fill_color\": \"#d3dde5\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 2\n    }," +
                              "\n    {\n      \"order\": 5,\n      \"type_name\": \"railway\",\n      \"stroke_color\": \"#e2e4e7\",\n      \"fill_color\": \"#e2e4e7\",\n      \"fill_if_closed\": false,\n      \"stroke_thickness\": 4\n    }," +
                              "\n    {\n      \"order\": 6,\n      \"type_name\": \"building\",\n      \"stroke_color\": \"#e8e9ed\",\n      \"fill_color\": \"#e8e9ed\",\n      \"fill_if_closed\": true,\n      \"stroke_thickness\": 1\n    }" +
                              "\n  ]\n}";
    }
}