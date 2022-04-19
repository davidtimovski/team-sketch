using System.Collections.Generic;
using Avalonia.Media;
using TeamSketch.Models;

namespace TeamSketch.Utils;

public static class BrushSettings
{
    private static readonly Dictionary<byte, Color> colorLookup = new(5)
    {
        { 0b_0000, Color.FromRgb(34, 34, 34) },
        { 0b_0001, Color.FromRgb(255, 255, 255) },
        { 0b_0010, Color.FromRgb(235, 51, 36) },
        { 0b_0011, Color.FromRgb(0, 162, 232) },
        { 0b_0100, Color.FromRgb(34, 177, 76) }
    };
    private static readonly Dictionary<byte, double> thicknessLookup = new(6)
    {
        { 0b_0000, 2 },
        { 0b_0001, 4 },
        { 0b_0010, 6 },
        { 0b_0011, 8 },
        { 0b_0100, 10 },
        { 0b_0101, 50 }
    };

    public static ColorsEnum BrushColor { get; set; }
    public static ThicknessEnum Thickness { get; set; }

    public static SolidColorBrush GetColorBrush(byte color)
    {
        return new SolidColorBrush(colorLookup[color]);
    }

    public static SolidColorBrush GetColorBrush()
    {
        return new SolidColorBrush(colorLookup[(byte)BrushColor]);
    }

    public static double GetThicknessNumber(byte thickness)
    {
        return thicknessLookup[thickness];
    }

    public static double GetThicknessNumber()
    {
        return thicknessLookup[(byte)Thickness];
    }
}
