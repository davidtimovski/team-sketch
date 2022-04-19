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
        var thickness = BrushSettings.GetThicknessNumber();
        var ellipse = new Ellipse
        {
            Margin = new Thickness(x - (thickness / 2), y - (thickness / 2), 0, 0),
            Fill = BrushSettings.GetColorBrush(),
            Width = thickness,
            Height = thickness
        };
        _canvas.Children.Add(ellipse);
    }

    public void DrawLine(double x1, double y1, double x2, double y2)
    {
        var thickness = BrushSettings.GetThicknessNumber();
        var colorBrush = BrushSettings.GetColorBrush();

        var ellipse = new Ellipse
        {
            Margin = new Thickness(x1 - (thickness / 2), y1 - (thickness / 2), 0, 0),
            Fill = colorBrush,
            Width = thickness,
            Height = thickness
        };
        _canvas.Children.Add(ellipse);

        Line line = new();
        line.StrokeThickness = thickness;
        line.Stroke = colorBrush;
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
}
