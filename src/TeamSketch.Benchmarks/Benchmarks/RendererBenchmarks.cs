using Avalonia.Controls;
using BenchmarkDotNet.Attributes;
using TeamSketch.Services;

namespace TeamSketch.Benchmarks
{
    [SimpleJob(launchCount: 5, warmupCount: 5, targetCount: 5)]
    [MemoryDiagnoser]
    public class RendererBenchmarks
    {
        private readonly Random _random = new();
        private readonly int x1;
        private readonly int y1;
        private readonly int x2;
        private readonly int y2;
        private readonly IRenderer _renderer;

        public RendererBenchmarks()
        {
            var canvas = new Canvas
            {
                Width = Globals.CanvasWidth,
                Height = Globals.CanvasHeight
            };
            _renderer = new Renderer(canvas);

            x1 = _random.Next(0, Globals.CanvasWidth);
            y1 = _random.Next(0, Globals.CanvasHeight);
            x2 = _random.Next(0, Globals.CanvasWidth);
            y2 = _random.Next(0, Globals.CanvasHeight);
        }

        [Benchmark]
        public void DrawLine() => _renderer.DrawLine(x1, y1, x2, y2);
    }
}
