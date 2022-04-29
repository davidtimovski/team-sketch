using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
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
    private readonly BrushSettings _brushSettings;
    private readonly Canvas _canvas;

    public Renderer(BrushSettings brushSettings, Canvas canvas)
    {
        _brushSettings = brushSettings;
        _canvas = canvas;
    }

    public void DrawPoint(double x, double y)
    {
        var ellipse = new Ellipse
        {
            Margin = new Thickness(x - _brushSettings.HalfThickness, y - _brushSettings.HalfThickness, 0, 0),
            Fill = _brushSettings.ColorBrush,
            Width = _brushSettings.Thickness,
            Height = _brushSettings.Thickness
        };
        _canvas.Children.Add(ellipse);
    }

    public void DrawLine(double x1, double y1, double x2, double y2)
    {
        var ellipse = new Ellipse
        {
            Margin = new Thickness(x1 - _brushSettings.HalfThickness, y1 - _brushSettings.HalfThickness, 0, 0),
            Fill = _brushSettings.ColorBrush,
            Width = _brushSettings.Thickness,
            Height = _brushSettings.Thickness
        };
        _canvas.Children.Add(ellipse);

        Line line = new();
        line.StrokeThickness = _brushSettings.Thickness;
        line.StartPoint = new Point(x1, y1);
        line.EndPoint = new Point(x2, y2);
        line.Stroke = _brushSettings.ColorBrush;

        _canvas.Children.Add(line);
    }

    public (double x, double y) RestrictPointToCanvas(double x, double y)
    {
        if (x > _brushSettings.MaxBrushPointX)
        {
            x = _brushSettings.MaxBrushPointX;
        }
        else if (x < _brushSettings.MinBrushPoint)
        {
            x = _brushSettings.MinBrushPoint;
        }

        if (y > _brushSettings.MaxBrushPointY)
        {
            y = _brushSettings.MaxBrushPointY;
        }
        else if (y < _brushSettings.MinBrushPoint)
        {
            y = _brushSettings.MinBrushPoint;
        }

        return (x, y);
    }
}
