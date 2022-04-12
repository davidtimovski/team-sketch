using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Common;

namespace TeamSketch.Services;

public interface IRenderer
{
    ColorsEnum BrushColor { get; }
    double BrushThickness { get; }
    void SetBrush(double thickness, ColorsEnum color);
    void SetEraser();
    void Draw(PointDto point, bool remote = false);
    void Draw(LineDto line, bool remote = false);
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
    public double BrushThickness { get; private set; } = 2;

    public void SetBrush(double thickness, ColorsEnum color)
    {
        BrushThickness = thickness;
        BrushColor = color;
    }

    public void SetEraser()
    {
        BrushThickness = 50;
        BrushColor = ColorsEnum.Eraser;
    }

    public void Draw(PointDto point, bool remote = false)
    {
        var ellipse = new Ellipse
        {
            Margin = new Thickness(point.X - (point.Size / 2), point.Y - (point.Size / 2), 0, 0),
            Fill = GetBrush(point.Color),
            Width = point.Size,
            Height = point.Size
        };
        _canvas.Children.Add(ellipse);

        if (remote)
        {
            return;
        }

        try
        {
            _ = _signalRService.DrawAsync(point);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    public void Draw(LineDto line, bool remote = false)
    {
        Line lineShape = new();
        lineShape.StrokeThickness = line.Thickness;
        lineShape.Stroke = GetBrush(line.Color);
        lineShape.StartPoint = new Point(line.X1, line.Y1);
        lineShape.EndPoint = new Point(line.X2, line.Y2);
        _canvas.Children.Add(lineShape);

        if (remote)
        {
            return;
        }

        try
        {
            _ = _signalRService.DrawAsync(line);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
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
