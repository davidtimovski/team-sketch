using BenchmarkDotNet.Attributes;
using TeamSketch.Models;
using TeamSketch.Services;
using TeamSketch.Utils;

namespace TeamSketch.Benchmarks
{
    [SimpleJob(launchCount: 5, warmupCount: 5, targetCount: 5)]
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

    [SimpleJob(launchCount: 5, warmupCount: 5, targetCount: 5)]
    [MemoryDiagnoser]
    public class PayloadConverterLineBenchmarks
    {
        private readonly Random _random = new();
        private readonly LineDrawSegment[] _lineSegments = new LineDrawSegment[20];

        public PayloadConverterLineBenchmarks()
        {
            for (int i = 0; i < _lineSegments.Length; i++)
            {
                var lineX1 = _random.Next(0, Globals.CanvasWidth);
                var lineY1 = _random.Next(0, Globals.CanvasHeight);
                var lineX2 = _random.Next(0, Globals.CanvasWidth);
                var lineY2 = _random.Next(0, Globals.CanvasHeight);
                _lineSegments[i] = new LineDrawSegment(lineX1, lineY1, lineX2, lineY2);
            }
        }

        [Benchmark]
        public void LineToBytes() => PayloadConverter.ToBytes(_lineSegments, ThicknessEnum.SemiThin, ColorsEnum.Blue);
    }
}
