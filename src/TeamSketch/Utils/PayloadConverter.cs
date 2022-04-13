using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using TeamSketch.Models;

namespace TeamSketch
{
    public static class PayloadConverter
    {
        public static byte[] PointToBytes(double x, double y, short size, ColorsEnum color)
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

        public static Ellipse BytesToPoint(byte[] bytes)
        {
            var x = (bytes[1] << 8) + bytes[0];
            var y = (bytes[3] << 8) + bytes[2];
            var size = bytes[4];
            var color = bytes[5];

            var ellipse = new Ellipse
            {
                Margin = new Thickness(x - (size / 2), y - (size / 2), 0, 0),
                Fill = ColorByteToBrush(color),
                Width = size,
                Height = size
            };

            return ellipse;
        }

        public static byte[] LineToBytes(double x1, double y1, double x2, double y2, short thickness, ColorsEnum color)
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

        public static Line BytesToLine(byte[] bytes)
        {
            var x1 = (bytes[1] << 8) + bytes[0];
            var y1 = (bytes[3] << 8) + bytes[2];
            var x2 = (bytes[5] << 8) + bytes[4];
            var y2 = (bytes[7] << 8) + bytes[6];
            var thickness = bytes[8];
            var color = bytes[9];

            Line result = new();
            result.StrokeThickness = thickness;
            result.StartPoint = new Point(x1, y1);
            result.EndPoint = new Point(x2, y2);
            result.Stroke = ColorByteToBrush(color);

            return result;
        }

        private static SolidColorBrush ColorByteToBrush(byte color)
        {
            return color switch
            {
                0b_0000 => new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                0b_0001 => new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                0b_0010 => new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                0b_0011 => new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                0b_0100 => new SolidColorBrush(Color.FromRgb(0, 255, 0)),
                _ => throw new ArgumentException(null, nameof(color))
            };
        }
    }
}
