using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using MapBuilder.Api.Controllers;

namespace MapBuilder.Web.Components.Pages;

public partial class GetMap
{
    public BECanvas MapCanvas;
    private Canvas2DContext _context;
    public double Lat = 37.570199;
    public double Lng = -83.710665;
    public int Level = 13;
    private string _color = "green";
    private float _thickness;
    private bool _closed;
    private bool _filled;
    
    public DateTime TimeStarted;
    public DateTime TimeFinished;
    public string TimeInfo;

    public string Info = "";
    public string ProgressInfo = "";

    public bool ShowingGenerationSettings = false;
    public bool ShowingRegenerateButton = false;
    
    public async Task DrawMap()
    {
        
        TimeInfo = ""; 
        ProgressInfo = "";
        Info = "Getting data... (Takes the longest)";
        TimeStarted = DateTime.Now;
        _context = await MapCanvas.CreateCanvas2DAsync();
        string instructions = await _drawController.Instructions(Level,Lat,Lng, Completion);
        Info = "Drawing map...";
        StateHasChanged();
        string[] commands = instructions.Split(';');
        await _context.SetFillStyleAsync("green");
        await _context.FillRectAsync(0, 0, 500, 500);
        foreach (string command in commands)
        {
            string[] splitted = command.Split(':');

            await DoCommand(splitted[0],splitted[1]);
        }
        TimeFinished = DateTime.Now;
        TimeInfo = " (Time: "+(TimeFinished - TimeStarted)+")";
        Info = "Finished!";
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
    public void SetColorAndThicknessFromType(string type)
    {
        Console.WriteLine(type);
        if (type.Contains("water"))
        {
            _thickness = 1f+((Level-8f)/2f);
            _color = "blue";
        }else if (type.Contains("building"))
        {
            _thickness = 1f+((Level-8f)/2f);
            _color = "orange";
        }else if (type.Contains("road"))
        {
            _thickness = 1f+((Level-8f)/2f);
            _color = "black";
        }
        else if (type.Contains("path"))
        {
            _thickness = 1f+((Level-8f)/2f);
            _color = "gray";
        }
        else
        {
            _thickness = 1f+((Level-8f)/3f);
            _color = "red";
        }
    }

    public void SetLatLon(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }

    public string Completion(int completed, int total)
    {
        ProgressInfo = $"Progress: {completed}/{total}";
        StateHasChanged();
        return ProgressInfo;
    }
    
    private readonly DrawController _drawController;
    
    public GetMap()
    {
        _drawController = new DrawController();
    }
}