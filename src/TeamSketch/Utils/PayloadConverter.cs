using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using TeamSketch.Models;
using TeamSketch.Utils;

namespace TeamSketch
{
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
            var size = BrushSettings.GetThicknessNumber(bytes[4]);
            var colorBrush = BrushSettings.GetColorBrush(bytes[5]);

            var ellipse = new Ellipse
            {
                Margin = new Thickness(x - (size / 2), y - (size / 2), 0, 0),
                Fill = colorBrush,
                Width = size,
                Height = size
            };

            return ellipse;
        }

        public static byte[] ToBytes(double x1, double y1, double x2, double y2, ThicknessEnum thickness, ColorsEnum color)
        {
            var bytes = new byte[10];

            bytes[0] = (byte)x1;
            bytes[1] = (byte)((short)x1 >> 8);

            bytes[2] = (byte)y1;
            bytes[3] = (byte)((short)y1 >> 8);

            bytes[4] = (byte)x2;
            bytes[5] = (byte)((short)x2 >> 8);

            bytes[6] = (byte)y2;
            bytes[7] = (byte)((short)y2 >> 8);

            bytes[8] = (byte)thickness;
            bytes[9] = (byte)color;

            return bytes;
        }

        public static IEnumerable<IControl> ToLineShapes(byte[] bytes)
        {
            var x1 = (bytes[1] << 8) + bytes[0];
            var y1 = (bytes[3] << 8) + bytes[2];
            var x2 = (bytes[5] << 8) + bytes[4];
            var y2 = (bytes[7] << 8) + bytes[6];
            var thickness = BrushSettings.GetThicknessNumber(bytes[8]);
            var colorBrush = BrushSettings.GetColorBrush(bytes[9]);

            var result = new List<IControl>(2);

            var ellipse = new Ellipse
            {
                Margin = new Thickness(x1 - (thickness / 2), y1 - (thickness / 2), 0, 0),
                Fill = colorBrush,
                Width = thickness,
                Height = thickness
            };
            result.Add(ellipse);

            Line line = new();
            line.StrokeThickness = thickness;
            line.StartPoint = new Point(x1, y1);
            line.EndPoint = new Point(x2, y2);
            line.Stroke = colorBrush;
            result.Add(line);

            return result;
        }
    }
}
