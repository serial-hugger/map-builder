using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MapBuilder.Web.Components.Pages;

public partial class DrawSettings
{
    [Inject]
    public WebService WebService {get; set;}

    public async Task SaveDrawSettings()
    {
        bool success = WebService.SaveDrawSettings();
        if (!success)
        {
            await JSRuntime.InvokeVoidAsync("alert","ERROR: Failed to save draw settings, make sure you entered a valid JSON.");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert","Saved successfully.");
            StateHasChanged();
        }
    }
}