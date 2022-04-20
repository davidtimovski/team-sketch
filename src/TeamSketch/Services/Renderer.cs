using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using TeamSketch.Utils;

namespace TeamSketch.Services;

public interface IRenderer
{
    void DrawPoint(double x, double y);
    void DrawLine(double x1, double y1, double x2, double y2);
    (double x, double y) RestrictPointToCanvas(double x, double y);
}

public class Renderer : IRenderer
{
    private readonly Canvas _canvas;

    public Renderer(Canvas canvas)
    {
        _canvas = canvas;
    }

    public void DrawPoint(double x, double y)
    {
        var ellipse = new Ellipse
        {
            Margin = new Thickness(x - BrushSettings.HalfThickness, y - BrushSettings.HalfThickness, 0, 0),
            Fill = BrushSettings.ColorBrush,
            Width = BrushSettings.Thickness,
            Height = BrushSettings.Thickness
        };
        _canvas.Children.Add(ellipse);
    }

    public void DrawLine(double x1, double y1, double x2, double y2)
    {
        var ellipse = new Ellipse
        {
            Margin = new Thickness(x1 - BrushSettings.HalfThickness, y1 - BrushSettings.HalfThickness, 0, 0),
            Fill = BrushSettings.ColorBrush,
            Width = BrushSettings.Thickness,
            Height = BrushSettings.Thickness
        };
        _canvas.Children.Add(ellipse);

        Line line = new();
        line.StrokeThickness = BrushSettings.Thickness;
        line.StartPoint = new Point(x1, y1);
        line.EndPoint = new Point(x2, y2);
        line.Stroke = BrushSettings.ColorBrush;

        _canvas.Children.Add(line);
    }

    public (double x, double y) RestrictPointToCanvas(double x, double y)
    {
        if (x > BrushSettings.MaxBrushPointX)
        {
            x = BrushSettings.MaxBrushPointX;
        }
        else if (x < BrushSettings.MinBrushPoint)
        {
            x = BrushSettings.MinBrushPoint;
        }

        if (y > BrushSettings.MaxBrushPointY)
        {
            y = BrushSettings.MaxBrushPointY;
        }
        else if (y < BrushSettings.MinBrushPoint)
        {
            y = BrushSettings.MinBrushPoint;
        }

        return (x, y);
    }
}
