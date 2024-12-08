using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using MapBuilder.Api.Controllers;
using MapBuilder.Shared.SerializationModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MapBuilder.Web.Components;

public partial class Generation
{
    [Parameter] 
    public string Type { get; set; }

    private BECanvas _mapCanvas;
    private Canvas2DContext _context;
    private double _lat = 37.570199;
    private double _lng = -83.710665;
    private int _level = 13;
    private string _outType = "json";
    private string _strokeColor = "green";
    private string _fillColor = "green";
    private float _thickness;
    private bool _closed;
    private bool _filled;

    private DateTime _timeStarted;
    private DateTime _timeFinished;
    private string _timeInfo;

    private string _info = "";
    private string _progressInfo = "";

    private bool _hidingGenerationSettings = false;
    private bool _hidingRegenerateButton = true;
    private bool _hidingData = true;
    private bool _hidingCanvas = true;
    private bool _hidingCancelButton = true;
    private string _cancelText = "Cancel";
    private bool _disablingCancelButton;

    private int _completed = 0;

    private string _dataDescription = "";
    private string _data = "";

    public CancellationTokenSource cts;
    
    [Inject]
    public WebService WebService {get; set;}
    
    public async Task DoAction()
    {
        await JSRuntime.InvokeVoidAsync("setNavMenuHidden",true);
        cts = new CancellationTokenSource();
        _hidingCancelButton = false;
        _completed = 0;
        _hidingGenerationSettings = true;
        _hidingRegenerateButton = true;
        _hidingData = true;
        _timeInfo = ""; 
        _progressInfo = "";
        _info = "Getting data... (Takes the longest)";
        StateHasChanged();
        _timeStarted = DateTime.Now;
        _context = await _mapCanvas.CreateCanvas2DAsync();
        if (Type == "map")
        {
            try
            {
                await DrawMap(cts.Token);
            }
            catch
            {
                Console.WriteLine("Cancelled");
            }
        }
        if (Type == "json")
        {
            try
            {
                await RetrieveData(cts.Token,WebService.FeatureSettings);
            }
            catch
            {
                Console.WriteLine("Cancelled");
            }
        }
        if (Type == "osm")
        {
            try
            {
                await RetrieveOsm(cts.Token);
            }
            catch
            {
                Console.WriteLine("Cancelled");
            }
        }
        _hidingCancelButton = true;
        await JSRuntime.InvokeVoidAsync("setNavMenuHidden", false);
        StateHasChanged();
    }
    
    public async Task RetrieveData(CancellationToken ct, FeatureSettings? featureSettings)
    {
        _hidingCanvas = true;
        MapController mapController = new MapController();
        _data = await mapController.GetMap(_level,_lat,_lng, Completion,cts.Token, featureSettings);
        _dataDescription = "JSON data:";
        _hidingData = false;
        _timeFinished = DateTime.Now;
        _timeInfo = " (Time: "+(_timeFinished - _timeStarted)+")";
        _info = "Finished!";
        _hidingGenerationSettings = true;
        _hidingRegenerateButton = false;
        StateHasChanged();
    }
    public async Task RetrieveOsm(CancellationToken ct)
    {
        string output = "";
        if (_outType == "json")
        {
            output = "[out:json]";
        }else if (_outType == "xml")
        {
            output = "[out:xml]";
        }else if (_outType == "csv")
        {
            output = "[out:csv(::id,::type,\"name\")]";
        }
        _hidingCanvas = true;
        OSMController osmController = new OSMController();
        _data = await osmController.GetData(output,_level,_lat,_lng,cts.Token);
        _dataDescription = "Raw OpenStreetMap data:";
        _hidingData = false;
        _timeFinished = DateTime.Now;
        _timeInfo = " (Time: "+(_timeFinished - _timeStarted)+")";
        _info = "Finished!";
        _hidingGenerationSettings = true;
        _hidingRegenerateButton = false;
        StateHasChanged();
    }

    public async Task CancelAction()
    {
        _cancelText = "Canceling...";
        _disablingCancelButton = true;
        StateHasChanged();
        await cts.CancelAsync();
        Regenerate();
    }
    public async Task DrawMap(CancellationToken ct)
    {
        _hidingCanvas = false;
        DrawInstructer drawInstructer = new DrawInstructer(WebService.FeatureSettings,WebService.DrawSettings);
        _data = await drawInstructer.Instructions(_level,_lat,_lng, Completion, cts.Token);
        _dataDescription = "Instructions used to draw the map:";
        _info = "Drawing map...";
        _hidingData = false;
        StateHasChanged();
        string[] commands = _data.Split(';');
        foreach (string command in commands)
        {
            string[] splitted = command.Split(':');

            await DoCommand(splitted[0],splitted[1]);
        }
        _timeFinished = DateTime.Now;
        _timeInfo = " (Time: "+(_timeFinished - _timeStarted)+")";
        _info = "Finished!";
        _hidingGenerationSettings = true;
        _hidingRegenerateButton = false;
        StateHasChanged();
    }
    async Task DoCommand(string key, string value)
    {
        if (key=="closed")
        {
            _closed = Convert.ToBoolean(value);
        }
        if (key=="filled")
        {
            _filled = Convert.ToBoolean(value);
        }
        if (key=="fill_color")
        {
            _fillColor = value;
        }
        if (key=="fill_base")
        {
            await _context.SetFillStyleAsync(value);
            await _context.FillRectAsync(0, 0, 500, 500);
        }
        if (key=="stroke_color")
        {
            _strokeColor = value;
        }
        if (key=="thickness")
        {
            _thickness = float.Parse(value);
        }
        if (key=="points")
        {
            await _context.MoveToAsync(0, 0);
            await _context.SetLineWidthAsync(_thickness +((_level-10f)/2f));
            await _context.SetStrokeStyleAsync(_strokeColor);
            await _context.SetFillStyleAsync(_fillColor);
            string[] points = value.Split(',');
            float x;
            float y;
            
            for (int i = 0; i < points.Length; i++)
            {
                if (i%2==1)
                {
                    float pointY = float.Parse(points[i-1])*2f;
                    float pointX = float.Parse(points[i])*2f;
                    y = 500-((pointY*250)+250);
                    x = ((pointX*250)+250);
                    if (i==1)
                    {
                        await _context.BeginPathAsync();
                        await _context.MoveToAsync(x, y);
                    }
                    else
                    {
                        await _context.LineToAsync(x, y);
                    }
                }
            }
            if (_closed)
            {
                await _context.ClosePathAsync();
            }
            if (_filled)
            {
                await _context.StrokeAsync();
                await _context.FillAsync();
            }
            else
            {
                await _context.StrokeAsync();
            }
        }
    }

    public void Regenerate()
    {
        _hidingGenerationSettings = false;
        _hidingRegenerateButton = true;
        _hidingData = true;
        _progressInfo = "";
        _info = "";
        _timeInfo = "";
        _data = "";
        _dataDescription = "";
        _cancelText = "Cancel";
        _disablingCancelButton = false;
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