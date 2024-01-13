using Avalonia;
using BenchmarkDotNet.Attributes;
using TeamSketch.Models;
using TeamSketch.Utils;

namespace TeamSketch.Benchmarks
{
    [SimpleJob(launchCount: 5, warmupCount: 5)]
    [MemoryDiagnoser]
    public class PayloadConverterPointBenchmarks
    {
        private readonly Random _random = new();
        private readonly int x1;
        private readonly int y1;

        public PayloadConverterPointBenchmarks()
        {
            x1 = _random.Next(0, Globals.CanvasWidth);
            y1 = _random.Next(0, Globals.CanvasHeight);
        }

        [Benchmark]
        public void PointToBytes() => PayloadConverter.ToBytes(x1, y1, ThicknessEnum.SemiThin, ColorsEnum.Blue);
    }

    [SimpleJob(launchCount: 5, warmupCount: 5)]
    [MemoryDiagnoser]
    public class PayloadConverterLineBenchmarks
    {
        private readonly Random _random = new();
        private readonly List<Point> _linePoints = new(40);

        public PayloadConverterLineBenchmarks()
        {
            for (int i = 0; i < _linePoints.Count; i++)
            {
                var x = _random.Next(0, Globals.CanvasWidth);
                var y = _random.Next(0, Globals.CanvasHeight);
                _linePoints[i] = new Point(x, y);
            }
        }

        [Benchmark]
        public void LineToBytes() => PayloadConverter.ToBytes(_linePoints, ThicknessEnum.SemiThin, ColorsEnum.Blue);
    }
}
