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
    double BrushThickness { get; set; }
    void SetBrushColor(ColorsEnum brushColor);
    void Draw(PointDto point, bool remote = false);
    void Draw(LineDto line, bool remote = false);
}

public class Renderer : IRenderer
{
    private readonly Canvas _canvas;
    private readonly ISignalRService _signalRService;
    private ISolidColorBrush _brush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

    public Renderer(Canvas canvas, ISignalRService signalRService)
    {
        _canvas = canvas;
        _signalRService = signalRService;
    }

    public ColorsEnum BrushColor { get; private set; }
    public double BrushThickness { get; set; } = 2;

    public void SetBrushColor(ColorsEnum color)
    {
        switch (color)
        {
            case ColorsEnum.Default:
                _brush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                break;
            case ColorsEnum.Red:
                _brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                break;
            case ColorsEnum.Blue:
                _brush = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                break;
            case ColorsEnum.Green:
                _brush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                break;
        }

        BrushColor = color;
    }

    public void Draw(PointDto point, bool remote = false)
    {
        var ellipse = new Ellipse
        {
            Margin = new Thickness(point.X - (BrushThickness / 2), point.Y - (BrushThickness / 2), 0, 0),
            Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
            Width = BrushThickness,
            Height = BrushThickness
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
        lineShape.StrokeThickness = BrushThickness;
        lineShape.Stroke = _brush;
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
}
