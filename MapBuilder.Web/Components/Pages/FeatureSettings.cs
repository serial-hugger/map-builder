using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MapBuilder.Web.Components.Pages;

public partial class FeatureSettings
{
    [Inject]
    public WebService WebService {get; set;}

    public async Task SaveFeatureSettings()
    {
        bool success = WebService.SaveFeatureSettings();
        if (!success)
        {
            await JSRuntime.InvokeVoidAsync("popup","ERROR: Failed to save feature settings, make sure you entered a valid JSON.");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("popup","Saved successfully.");
            StateHasChanged();
        }
    }
    public void LoadModes(Action loadModeFunction)
    {
        loadModeFunction();
        StateHasChanged();
    }
}