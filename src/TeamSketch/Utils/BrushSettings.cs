using System.Collections.Generic;
using Avalonia.Media;
using TeamSketch.Models;

namespace TeamSketch.Utils;

public static class BrushSettings
{
    private static readonly Dictionary<ColorsEnum, SolidColorBrush> colorLookup = new(5)
    {
        { ColorsEnum.Default, new SolidColorBrush(Color.FromRgb(34, 34, 34)) },
        { ColorsEnum.Eraser, new SolidColorBrush(Color.FromRgb(255, 255, 255)) },
        { ColorsEnum.Red, new SolidColorBrush(Color.FromRgb(235, 51, 36)) },
        { ColorsEnum.Blue, new SolidColorBrush(Color.FromRgb(0, 162, 232)) },
        { ColorsEnum.Green, new SolidColorBrush(Color.FromRgb(34, 177, 76)) },
        { ColorsEnum.Yellow, new SolidColorBrush(Color.FromRgb(255, 242, 0)) },
        { ColorsEnum.Orange, new SolidColorBrush(Color.FromRgb(255, 127, 39)) },
        { ColorsEnum.Purple, new SolidColorBrush(Color.FromRgb(163, 73, 164)) },
        { ColorsEnum.Pink, new SolidColorBrush(Color.FromRgb(255, 174, 201)) },
        { ColorsEnum.Gray, new SolidColorBrush(Color.FromRgb(195, 195, 195)) }
    };
    private static readonly Dictionary<ThicknessEnum, double> thicknessLookup = new(6)
    {
        { ThicknessEnum.Thin, 2 },
        { ThicknessEnum.SemiThin, 4 },
        { ThicknessEnum.Medium, 6 },
        { ThicknessEnum.SemiThick, 8 },
        { ThicknessEnum.Thick, 10 },
        { ThicknessEnum.Eraser, 50 }
    };

    private static ColorsEnum brushColor;
    public static ColorsEnum BrushColor
    {
        get => brushColor;
        set
        {
            brushColor = value;
            ColorBrush = colorLookup[value];
        }
    }
    public static SolidColorBrush ColorBrush { get; private set; } = colorLookup[ColorsEnum.Default];

    private static ThicknessEnum brushThickness = ThicknessEnum.SemiThin;
    public static ThicknessEnum BrushThickness
    {
        get => brushThickness; 
        set
        {
            brushThickness = value;
            Thickness = thicknessLookup[value];
            HalfThickness = Thickness / 2;

            MaxBrushPointX = Globals.CanvasWidth - HalfThickness;
            MaxBrushPointY = Globals.CanvasHeight - HalfThickness;
            MinBrushPoint = HalfThickness;
        }
    }
    public static double Thickness { get; private set; } = thicknessLookup[brushThickness];
    public static double HalfThickness { get; private set; } = thicknessLookup[brushThickness] / 2;

    public static double MaxBrushPointX { get; private set; } = Globals.CanvasWidth - thicknessLookup[brushThickness] / 2;
    public static double MaxBrushPointY { get; private set; } = Globals.CanvasHeight - thicknessLookup[brushThickness] / 2;
    public static double MinBrushPoint { get; private set; } = thicknessLookup[brushThickness] / 2;

    public static SolidColorBrush FindColorBrush(byte color)
    {
        return colorLookup[(ColorsEnum)color];
    }

    public static double FindThickness(byte thickness)
    {
        return thicknessLookup[(ThicknessEnum)thickness];
    }
}
