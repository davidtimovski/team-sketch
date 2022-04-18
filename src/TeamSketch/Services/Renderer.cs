using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using TeamSketch.Models;

namespace TeamSketch.Services;

public interface IRenderer
{
    void DrawPoint(double x, double y);
    void DrawLine(double x1, double y1, double x2, double y2);
    (double x, double y) RestrictPointToCanvas(double x, double y);
}

public class Renderer : IRenderer
{
    private readonly IBrushService _brushService;
    private readonly Canvas _canvas;

    public Renderer(IBrushService brushService, Canvas canvas)
    {
        _brushService = brushService;
        _canvas = canvas;
    }

    public void DrawPoint(double x, double y)
    {
        var thickness = GetBrushThickness(_brushService.Thickness);
        var ellipse = new Ellipse
        {
            Margin = new Thickness(x - (thickness / 2), y - (thickness / 2), 0, 0),
            Fill = GetBrushColor(_brushService.Color),
            Width = thickness,
            Height = thickness
        };
        _canvas.Children.Add(ellipse);
    }

    public void DrawLine(double x1, double y1, double x2, double y2)
    {
        Line line = new();
        line.StrokeThickness = GetBrushThickness(_brushService.Thickness);
        line.Stroke = GetBrushColor(_brushService.Color);
        line.StartPoint = new Point(x1, y1);
        line.EndPoint = new Point(x2, y2);
        _canvas.Children.Add(line);
    }

    public (double x, double y) RestrictPointToCanvas(double x, double y)
    {
        if (x > 1280)
        {
            x = 1280;
        }
        else if (x < 0)
        {
            x = 0;
        }

        if (y > 720)
        {
            y = 720;
        }
        else if (y < 0)
        {
            y = 0;
        }

        return (x, y);
    }

    private static SolidColorBrush GetBrushColor(ColorsEnum color)
    {
        return color switch
        {
            ColorsEnum.Default => new SolidColorBrush(Color.FromRgb(34, 34, 34)),
            ColorsEnum.Eraser => new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            ColorsEnum.Red => new SolidColorBrush(Color.FromRgb(235, 51, 36)),
            ColorsEnum.Blue => new SolidColorBrush(Color.FromRgb(0, 162, 232)),
            ColorsEnum.Green => new SolidColorBrush(Color.FromRgb(34, 177, 76)),
            _ => throw new ArgumentException(null, nameof(color))
        };
    }

    private static double GetBrushThickness(ThicknessEnum thickness)
    {
        return thickness switch
        {
            ThicknessEnum.Thin => 2,
            ThicknessEnum.SemiThin => 4,
            ThicknessEnum.Medium => 6,
            ThicknessEnum.SemiThick => 8,
            ThicknessEnum.Thick => 10,
            ThicknessEnum.Eraser => 50,
            _ => throw new ArgumentException(null, nameof(thickness))
        };
    }
}
