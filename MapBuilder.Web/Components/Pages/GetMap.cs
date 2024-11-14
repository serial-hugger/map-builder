using System.Drawing;
using Blazor.Extensions;
using MapBuilder.Api.Controllers;
using MapBuilder.Data;
using Microsoft.JSInterop;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;


namespace MapBuilder.Web.Components.Pages;

public partial class GetMap
{
    public BECanvas MapCanvas;
    private Canvas2DContext _context;
    public double Lat = 37.570199;
    public double Lng = -83.710665;
    private string _color = "green";
    private float _thickness = 0f;
    private bool _closed;
    private bool _filled;

    public string Info = "";
    
    public async Task DrawMap()
    {
        Info = "Getting instructions...";
        _context = await MapCanvas.CreateCanvas2DAsync();
        string instructions = await _drawController.Instructions(Lat,Lng);
        Info = "Executing instructions...";
        string[] commands = instructions.Split(';');
        await _context.SetFillStyleAsync("green");
        await _context.FillRectAsync(0, 0, 500, 500);
        foreach (string command in commands)
        {
            string[] splitted = command.Split(':');

            await DoCommand(splitted[0],splitted[1]);
        }
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
            await _context.SetLineWidthAsync(_thickness);
            await _context.SetStrokeStyleAsync(_color);
            await _context.SetFillStyleAsync(_color);
            float[] points = value.Split(',').Select(float.Parse).ToArray();
            float x = 0;
            float y = 0;
            
            for (int i = 0; i < points.Length; i++)
            {
                if (i%2==0)
                {
                    y = 500-((points[i]*250)+250);
                }
                if (i%2==1)
                {
                    x = ((points[i]*250)+250);
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
            _thickness = 0.5f;
            _color = "blue";
        }else if (type.Contains("building"))
        {
            _thickness = 0.25f;
            _color = "orange";
        }else if (type.Contains("road"))
        {
            _thickness = 1f;
            _color = "black";
        }
        else if (type.Contains("path"))
        {
            _thickness = 0.75f;
            _color = "gray";
        }
        else
        {
            _thickness = 0.1f;
            _color = "red";
        }
    }
    
    private readonly DrawController _drawController;
    
    public GetMap()
    {
        _drawController = new DrawController();
    }
}