using Avalonia;
using Avalonia.Controls;
using BenchmarkDotNet.Attributes;
using TeamSketch.Models;
using TeamSketch.Services;

namespace TeamSketch.Benchmarks;

[SimpleJob(launchCount: 5, warmupCount: 5)]
[MemoryDiagnoser]
public class RendererBenchmarks
{
    private readonly Random _random = new();
    private readonly IRenderer _renderer;
    private const int LineSegmentsCount = 20;

    public RendererBenchmarks()
    {
        var canvas = new Canvas
        {
            Width = Globals.CanvasWidth,
            Height = Globals.CanvasHeight
        };
        _renderer = new Renderer(new BrushSettings(""), canvas);

        for (int i = 0; i < LineSegmentsCount; i++)
        {
            var x1 = _random.Next(0, Globals.CanvasWidth);
            var y1 = _random.Next(0, Globals.CanvasHeight);
            var x2 = _random.Next(0, Globals.CanvasWidth);
            var y2 = _random.Next(0, Globals.CanvasHeight);
            _renderer.EnqueueLineSegment(new Point(x1, y1), new Point(x2, y2));
        }
    }

    [Benchmark]
    public void RenderLine() => _renderer.RenderLine();
}
