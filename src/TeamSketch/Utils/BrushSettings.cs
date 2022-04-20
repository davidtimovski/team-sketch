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

    private static ColorsEnum brushColor;
    public static ColorsEnum BrushColor
    {
        get => brushColor;
        set
        {
            brushColor = value;
            ColorBrush = new SolidColorBrush(colorLookup[(byte)value]);
        }
    }
    public static SolidColorBrush ColorBrush { get; private set; } = new SolidColorBrush(colorLookup[0b_0000]);

    private static ThicknessEnum brushThickness;
    public static ThicknessEnum BrushThickness
    {
        get => brushThickness; 
        set
        {
            brushThickness = value;
            Thickness = thicknessLookup[(byte)value];
            HalfThickness = Thickness / 2;

            MaxBrushPointX = Globals.CanvasWidth - HalfThickness;
            MaxBrushPointY = Globals.CanvasHeight - HalfThickness;
            MinBrushPoint = HalfThickness;
        }
    }
    public static double Thickness { get; private set; } = thicknessLookup[0b_0000];
    public static double HalfThickness { get; private set; } = thicknessLookup[0b_0000] / 2;

    public static double MaxBrushPointX { get; private set; } = Globals.CanvasWidth - thicknessLookup[0b_0000] / 2;
    public static double MaxBrushPointY { get; private set; } = Globals.CanvasHeight - thicknessLookup[0b_0000] / 2;
    public static double MinBrushPoint { get; private set; } = thicknessLookup[0b_0000] / 2;

    public static SolidColorBrush FindColorBrush(byte color)
    {
        return new SolidColorBrush(colorLookup[color]);
    }

    public static double FindThickness(byte thickness)
    {
        return thicknessLookup[thickness];
    }
}
