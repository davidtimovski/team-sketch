using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using TeamSketch.Models;

namespace TeamSketch.Services;

public interface IRenderer
{
    void DrawPoint(double x, double y);
    void EnqueueLineSegment(Point point1, Point point2);

    /// <summary>
    /// Render line from locally batched points.
    /// </summary>
    /// <returns>The points making up the line that was rendered.</returns>
    List<Point> RenderLine();

    /// <summary>
    /// Render line from the input points.
    /// </summary>
    /// <param name="linePointsQueue"></param>
    /// <param name="thickness"></param>
    /// <param name="colorBrush"></param>
    void RenderLine(Queue<Point> linePointsQueue, double thickness, SolidColorBrush colorBrush);

    Point RestrictPointToCanvas(double x, double y);
}

public class Renderer : IRenderer
{
    private readonly BrushSettings _brushSettings;
    private readonly Canvas _canvas;
    private readonly Queue<Point> _linePointsQueue = new();

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

    public void EnqueueLineSegment(Point point1, Point point2)
    {
        _linePointsQueue.Enqueue(point1);
        _linePointsQueue.Enqueue(point2);
    }
    
    public List<Point> RenderLine()
    {
        if (_linePointsQueue.Count == 0)
        {
            return new List<Point>();
        }

        var myPointCollection = new Points();

        var result = _linePointsQueue.ToList();
        var firstPoint = _linePointsQueue.Dequeue();

        while (_linePointsQueue.Count > 0)
        {
            var point = _linePointsQueue.Dequeue();
            myPointCollection.Add(point);
        }

        var pathGeometry = new PathGeometry();
        var pathFigure = new PathFigure
        {
            Segments = new PathSegments
            {
                new PolyLineSegment
                {
                    Points = myPointCollection
                }
            },
            StartPoint = firstPoint,
            IsClosed = false
        };
        pathGeometry.Figures.Add(pathFigure);

        var path = new Path
        {
            Stroke = _brushSettings.ColorBrush,
            StrokeThickness = _brushSettings.Thickness,
            Data = pathGeometry
        };
        _canvas.Children.Add(path);

        var ellipse = new Ellipse
        {
            Margin = new Thickness(firstPoint.X - _brushSettings.HalfThickness, firstPoint.Y - _brushSettings.HalfThickness, 0, 0),
            Fill = _brushSettings.ColorBrush,
            Width = _brushSettings.Thickness,
            Height = _brushSettings.Thickness
        };
        _canvas.Children.Add(ellipse);

        return result;
    }

    public void RenderLine(Queue<Point> linePointsQueue, double thickness, SolidColorBrush colorBrush)
    {
        if (linePointsQueue.Count == 0)
        {
            return;
        }

        var myPointCollection = new Points();

        var firstPoint = linePointsQueue.Dequeue();

        while (linePointsQueue.Count > 0)
        {
            var point = linePointsQueue.Dequeue();
            myPointCollection.Add(point);
        }

        var pathGeometry = new PathGeometry();
        var pathFigure = new PathFigure
        {
            Segments = new PathSegments
            {
                new PolyLineSegment
                {
                    Points = myPointCollection
                }
            },
            StartPoint = firstPoint,
            IsClosed = false
        };
        pathGeometry.Figures.Add(pathFigure);

        var path = new Path
        {
            Stroke = colorBrush,
            StrokeThickness = thickness,
            Data = pathGeometry
        };
        _canvas.Children.Add(path);

        var ellipse = new Ellipse
        {
            Margin = new Thickness(firstPoint.X - thickness / 2, firstPoint.Y - thickness / 2, 0, 0),
            Fill = colorBrush,
            Width = thickness,
            Height = thickness
        };
        _canvas.Children.Add(ellipse);
    }

    public Point RestrictPointToCanvas(double x, double y)
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

        return new Point(x, y);
    }
}
