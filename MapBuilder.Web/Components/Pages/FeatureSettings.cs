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
            await JSRuntime.InvokeVoidAsync("alert","ERROR: Failed to save feature settings, make sure you entered a valid JSON.");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert","Saved successfully.");
            StateHasChanged();
        }
    }
}