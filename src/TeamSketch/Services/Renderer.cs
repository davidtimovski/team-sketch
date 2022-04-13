using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using TeamSketch.Models;

namespace TeamSketch.Services;

public interface IRenderer
{
    ColorsEnum BrushColor { get; }
    short BrushThickness { get; }
    void SetBrush(short thickness, ColorsEnum color);
    void SetEraser();
    void DrawPoint(double x, double y);
    void DrawPointRemote(byte[] data);
    void DrawLine(double x1, double y1, double x2, double y2);
    void DrawLineRemote(byte[] data);
}

public class Renderer : IRenderer
{
    private readonly Canvas _canvas;
    private readonly ISignalRService _signalRService;

    public Renderer(Canvas canvas, ISignalRService signalRService)
    {
        _canvas = canvas;
        _signalRService = signalRService;
    }

    public ColorsEnum BrushColor { get; private set; }
    public short BrushThickness { get; private set; } = 2;

    public void SetBrush(short thickness, ColorsEnum color)
    {
        BrushThickness = thickness;
        BrushColor = color;
    }

    public void SetEraser()
    {
        BrushThickness = 50;
        BrushColor = ColorsEnum.Eraser;
    }

    public void DrawPoint(double x, double y)
    {
        var ellipse = new Ellipse
        {
            Margin = new Thickness(x - (BrushThickness / 2), y - (BrushThickness / 2), 0, 0),
            Fill = GetBrush(BrushColor),
            Width = BrushThickness,
            Height = BrushThickness
        };
        _canvas.Children.Add(ellipse);

        try
        {
            _ = _signalRService.DrawPointAsync(x, y, BrushThickness, BrushColor);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    public void DrawPointRemote(byte[] data)
    {
        var point = PayloadConverter.BytesToPoint(data);
        _canvas.Children.Add(point);
    }

    public void DrawLine(double x1, double y1, double x2, double y2)
    {
        Line line = new();
        line.StrokeThickness = BrushThickness;
        line.Stroke = GetBrush(BrushColor);
        line.StartPoint = new Point(x1, y1);
        line.EndPoint = new Point(x2, y2);

        _canvas.Children.Add(line);

        try
        {
            _ = _signalRService.DrawLineAsync(x1, y1, x2, y2, BrushThickness, BrushColor);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    public void DrawLineRemote(byte[] data)
    {
        var line = PayloadConverter.BytesToLine(data);
        _canvas.Children.Add(line);
    }

    private static SolidColorBrush GetBrush(ColorsEnum color)
    {
        return color switch
        {
            ColorsEnum.Default => new SolidColorBrush(Color.FromRgb(0, 0, 0)),
            ColorsEnum.Eraser => new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            ColorsEnum.Red => new SolidColorBrush(Color.FromRgb(255, 0, 0)),
            ColorsEnum.Blue => new SolidColorBrush(Color.FromRgb(0, 0, 255)),
            ColorsEnum.Green => new SolidColorBrush(Color.FromRgb(0, 255, 0)),
            _ => throw new ArgumentException(null, nameof(color))
        };
    }
}
