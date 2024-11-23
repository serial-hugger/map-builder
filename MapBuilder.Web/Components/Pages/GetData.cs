using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using MapBuilder.Api.Controllers;
using Microsoft.AspNetCore.Components;

namespace MapBuilder.Web.Components.Pages;

public partial class GetData
{
    private BECanvas _mapCanvas;
    private Canvas2DContext _context;
    
    private double _lat = 37.570199;
    private double _lng = -83.710665;
    private int _level = 13;

    private DateTime _timeStarted;
    private DateTime _timeFinished;
    private string _timeInfo;

    private string _data = "";

    private string _info = "";
    private string _progressInfo = "";

    private bool _hidingGenerationSettings = false;
    private bool _hidingRegenerateButton = true;
    private bool _hidingMap = true;

    private int _completed = 0;
    public async Task RetrieveData()
    {
        MapController _mapController = new MapController();
        _completed = 0;
        _hidingGenerationSettings = true;
        _hidingRegenerateButton = true;
        _hidingMap = true;
        _timeInfo = ""; 
        _progressInfo = "";
        _info = "Getting data... (Takes the longest)";
        StateHasChanged();
        _timeStarted = DateTime.Now;
        _context = await _mapCanvas.CreateCanvas2DAsync();
        _data = await _mapController.GetMap(_level,_lat,_lng, Completion);
        _hidingMap = false;
        _timeFinished = DateTime.Now;
        _timeInfo = " (Time: "+(_timeFinished - _timeStarted)+")";
        _info = "Finished!";
        _hidingGenerationSettings = true;
        _hidingRegenerateButton = false;
        StateHasChanged();
    }

    public void Regenerate()
    {
        _hidingGenerationSettings = false;
        _hidingRegenerateButton = true;
        _hidingMap = true;
        _progressInfo = "";
        _info = "";
        _timeInfo = "";
        StateHasChanged();
    }
    public void SetLatLon(double lat, double lng)
    {
        _lat = lat;
        _lng = lng;
    }
    public string Completion(int total)
    {
        _completed++;
        _progressInfo = $"Progress: {_completed}/{total}";
        StateHasChanged();
        return _progressInfo;
    }
}