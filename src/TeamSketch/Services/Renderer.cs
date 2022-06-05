using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using TeamSketch.Models;

namespace TeamSketch.Services;

public interface IRenderer
{
    void DrawPoint(double x, double y);
    void EnqueueLineSegment(LineDrawSegment segment);

    /// <summary>
    /// Render line from locally batched segments.
    /// </summary>
    /// <returns>A list of the rendered segments.</returns>
    LineDrawSegment[] RenderLine();

    /// <summary>
    /// Render line from remotely batched segments.
    /// </summary>
    /// <param name="lineSegmentsQueue"></param>
    /// <param name="thickness"></param>
    /// <param name="colorBrush"></param>
    void RenderLine(Queue<LineDrawSegment> lineSegmentsQueue, double thickness, SolidColorBrush colorBrush);

    (double x, double y) RestrictPointToCanvas(double x, double y);
}

public class Renderer : IRenderer
{
    private readonly BrushSettings _brushSettings;
    private readonly Canvas _canvas;
    private readonly Queue<LineDrawSegment> _lineSegmentsQueue = new();

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

    public void EnqueueLineSegment(LineDrawSegment segment)
    {
        _lineSegmentsQueue.Enqueue(segment);
    }
    
    public LineDrawSegment[] RenderLine()
    {
        if (_lineSegmentsQueue.Count == 0)
        {
            return Array.Empty<LineDrawSegment>();
        }

        var myPointCollection = new Points();

        var result = _lineSegmentsQueue.ToArray();
        var firstLine = _lineSegmentsQueue.Dequeue();

        while (_lineSegmentsQueue.Count > 0)
        {
            var line = _lineSegmentsQueue.Dequeue();

            myPointCollection.Add(new Point(line.X1, line.Y1));
            myPointCollection.Add(new Point(line.X2, line.Y2));
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
            StartPoint = new Point(firstLine.X1, firstLine.Y1),
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
            Margin = new Thickness(firstLine.X1 - _brushSettings.HalfThickness, firstLine.Y1 - _brushSettings.HalfThickness, 0, 0),
            Fill = _brushSettings.ColorBrush,
            Width = _brushSettings.Thickness,
            Height = _brushSettings.Thickness
        };
        _canvas.Children.Add(ellipse);

        return result;
    }

    public void RenderLine(Queue<LineDrawSegment> lineSegmentsQueue, double thickness, SolidColorBrush colorBrush)
    {
        if (lineSegmentsQueue.Count == 0)
        {
            return;
        }

        var myPointCollection = new Points();

        var firstLine = lineSegmentsQueue.Dequeue();

        while (lineSegmentsQueue.Count > 0)
        {
            var line = lineSegmentsQueue.Dequeue();

            myPointCollection.Add(new Point(line.X1, line.Y1));
            myPointCollection.Add(new Point(line.X2, line.Y2));
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
            StartPoint = new Point(firstLine.X1, firstLine.Y1),
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
            Margin = new Thickness(firstLine.X1 - thickness / 2, firstLine.Y1 - thickness / 2, 0, 0),
            Fill = colorBrush,
            Width = thickness,
            Height = thickness
        };
        _canvas.Children.Add(ellipse);
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

public record LineDrawSegment(double X1, double Y1, double X2, double Y2);
