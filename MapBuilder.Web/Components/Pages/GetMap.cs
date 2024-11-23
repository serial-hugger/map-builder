using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using MapBuilder.Api.Controllers;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace MapBuilder.Web.Components.Pages;

public partial class GetMap
{
    private BECanvas _mapCanvas;
    private Canvas2DContext _context;
    private double _lat = 37.570199;
    private double _lng = -83.710665;
    private int _level = 13;
    private string _color = "green";
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
    private bool _hidingMap = true;

    private int _completed = 0;

    private string _instructions = "";
    
    public async Task DrawMap()
    {
        DrawInstructer drawInstructer = new DrawInstructer();
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
        _instructions = await drawInstructer.Instructions(_level,_lat,_lng, Completion);
        _info = "Drawing map...";
        _hidingMap = false;
        StateHasChanged();
        string[] commands = _instructions.Split(';');
        await _context.SetFillStyleAsync("green");
        await _context.FillRectAsync(0, 0, 500, 500);
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
        if (key=="type")
        {
            SetColorAndThicknessFromType(value);
        }
        if (key=="closed")
        {
            _closed = Convert.ToBoolean(value);
        }
        if (key=="filled")
        {
            _filled = Convert.ToBoolean(value);
        }
        if (key=="points")
        {
            await _context.MoveToAsync(0, 0);
            await _context.SetLineWidthAsync(_thickness);
            await _context.SetStrokeStyleAsync(_color);
            await _context.SetFillStyleAsync(_color);
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
        _hidingMap = true;
        _progressInfo = "";
        _info = "";
        _timeInfo = "";
        _instructions = "";
        StateHasChanged();
    }
    public void SetColorAndThicknessFromType(string type)
    {
        Console.WriteLine(type);
        if (type.Contains("water"))
        {
            _thickness = 1f+((_level-8f)/2f);
            _color = "blue";
        }else if (type.Contains("building"))
        {
            _thickness = 1f+((_level-8f)/2f);
            _color = "orange";
        }else if (type.Contains("road"))
        {
            _thickness = 1f+((_level-8f)/2f);
            _color = "black";
        }
        else if (type.Contains("path"))
        {
            _thickness = 1f+((_level-8f)/2f);
            _color = "gray";
        }
        else
        {
            _thickness = 1f+((_level-8f)/3f);
            _color = "red";
        }
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