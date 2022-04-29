using BenchmarkDotNet.Attributes;
using TeamSketch.Models;
using TeamSketch.Utils;

namespace TeamSketch.Benchmarks
{
    [SimpleJob(launchCount: 5, warmupCount: 5, targetCount: 5)]
    [MemoryDiagnoser]
    public class PayloadConverterBenchmarks
    {
        private readonly Random _random = new();
        private readonly int x1;
        private readonly int y1;
        private readonly int x2;
        private readonly int y2;

        public PayloadConverterBenchmarks()
        {
            x1 = _random.Next(0, Globals.CanvasWidth);
            y1 = _random.Next(0, Globals.CanvasHeight);
            x2 = _random.Next(0, Globals.CanvasWidth);
            y2 = _random.Next(0, Globals.CanvasHeight);
        }

        [Benchmark]
        public void PointToBytes() => PayloadConverter.ToBytes(x1, y1, ThicknessEnum.SemiThin, ColorsEnum.Blue);

        [Benchmark]
        public void LineToBytes() => PayloadConverter.ToBytes(x1, y1, x2, y2, ThicknessEnum.SemiThin, ColorsEnum.Blue);
    }
}
