using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using TeamSketch.Models;
using TeamSketch.Services;

namespace TeamSketch.Utils;

public static class PayloadConverter
{
    public static byte[] ToBytes(double x, double y, ThicknessEnum size, ColorsEnum color)
    {
        var bytes = new byte[6];

        bytes[0] = (byte)x;
        bytes[1] = (byte)((short)x >> 8);

        bytes[2] = (byte)y;
        bytes[3] = (byte)((short)y >> 8);

        bytes[4] = (byte)size;
        bytes[5] = (byte)color;

        return bytes;
    }

    public static Ellipse ToPoint(byte[] bytes)
    {
        var x = (bytes[1] << 8) + bytes[0];
        var y = (bytes[3] << 8) + bytes[2];
        var size = BrushSettings.FindThickness(bytes[4]);
        var colorBrush = BrushSettings.FindColorBrush(bytes[5]);

        var ellipse = new Ellipse
        {
            Margin = new Thickness(x - (size / 2), y - (size / 2), 0, 0),
            Fill = colorBrush,
            Width = size,
            Height = size
        };

        return ellipse;
    }

    public static byte[] ToBytes(LineDrawSegment[] segments, ThicknessEnum thickness, ColorsEnum color)
    {
        var bytes = new byte[1 + segments.Length * 8 + 2];
        int currentIndex = 0;

        bytes[currentIndex++] = (byte)segments.Length;

        foreach (var segment in segments)
        {
            bytes[currentIndex++] = (byte)segment.X1;
            bytes[currentIndex++] = (byte)((short)segment.X1 >> 8);

            bytes[currentIndex++] = (byte)segment.Y1;
            bytes[currentIndex++] = (byte)((short)segment.Y1 >> 8);

            bytes[currentIndex++] = (byte)segment.X2;
            bytes[currentIndex++] = (byte)((short)segment.X2 >> 8);

            bytes[currentIndex++] = (byte)segment.Y2;
            bytes[currentIndex++] = (byte)((short)segment.Y2 >> 8);
        }

        bytes[currentIndex++] = (byte)thickness;
        bytes[currentIndex] = (byte)color;

        return bytes;
    }

    public static (Queue<LineDrawSegment> segments, double thickness, SolidColorBrush colorBrush) ToLine(byte[] bytes)
    {
        int currentIndex = 0;
        var count = (int)bytes[currentIndex++];

        var result = new Queue<LineDrawSegment>(count);

        for (var i = 0; i < count; i++)
        {
            var buffer = i * 8;

            var x1 = (bytes[buffer + 2] << 8) + bytes[buffer + 1];
            var y1 = (bytes[buffer + 4] << 8) + bytes[buffer + 3];
            var x2 = (bytes[buffer + 6] << 8) + bytes[buffer + 5];
            var y2 = (bytes[buffer + 8] << 8) + bytes[buffer + 7];
            
            result.Enqueue(new LineDrawSegment(x1, y1, x2, y2));
        }

        var size = BrushSettings.FindThickness(bytes[^2]);
        var colorBrush = BrushSettings.FindColorBrush(bytes[^1]);

        return (result, size, colorBrush);
    }
}
