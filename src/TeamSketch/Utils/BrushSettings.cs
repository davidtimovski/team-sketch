using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using TeamSketch.Models;

namespace TeamSketch.Utils;

public static class BrushSettings
{
    private const string CursorsPath = "avares://TeamSketch/Assets/Images/Cursors";
    private static readonly IAssetLoader assetLoader;
    private static readonly Dictionary<ColorsEnum, SolidColorBrush> colorLookup = new()
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
    private static readonly Dictionary<ThicknessEnum, double> thicknessLookup = new()
    {
        { ThicknessEnum.Thin, 2 },
        { ThicknessEnum.SemiThin, 4 },
        { ThicknessEnum.Medium, 6 },
        { ThicknessEnum.SemiThick, 8 },
        { ThicknessEnum.Thick, 10 },
        { ThicknessEnum.Eraser, 50 }
    };
    private static readonly Dictionary<ColorsEnum, string> cursorBrushPathLookup = new()
    {
        { ColorsEnum.Default, $"{CursorsPath}/brush-black.png" },
        { ColorsEnum.Red, $"{CursorsPath}/brush-red.png" },
        { ColorsEnum.Blue, $"{CursorsPath}/brush-blue.png" },
        { ColorsEnum.Green, $"{CursorsPath}/brush-green.png" },
        { ColorsEnum.Yellow, $"{CursorsPath}/brush-yellow.png" },
        { ColorsEnum.Orange, $"{CursorsPath}/brush-orange.png" },
        { ColorsEnum.Purple, $"{CursorsPath}/brush-purple.png" },
        { ColorsEnum.Pink, $"{CursorsPath}/brush-pink.png" },
        { ColorsEnum.Gray, $"{CursorsPath}/brush-gray.png" }
    };

    static BrushSettings()
    {
        assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();

        var path = new Uri(cursorBrushPathLookup[ColorsEnum.Default]);
        Cursor = new(new Bitmap(assetLoader.Open(path)), new PixelPoint(0, 0));
    }

    public static event EventHandler<BrushChangedEventArgs> BrushChanged;

    public static Cursor Cursor { get; private set; }

    private static ColorsEnum brushColor;
    public static ColorsEnum BrushColor
    {
        get => brushColor;
        set
        {
            brushColor = value;
            ColorBrush = colorLookup[value];

            if (value == ColorsEnum.Eraser)
            {
                var path = $"{CursorsPath}/eraser-{Thickness}.png";
                Cursor = new(new Bitmap(assetLoader.Open(new Uri(path))), new PixelPoint((int)HalfThickness, (int)HalfThickness));
            }
            else
            {
                var path = cursorBrushPathLookup[value];
                Cursor = new(new Bitmap(assetLoader.Open(new Uri(path))), new PixelPoint(0, 0));
            }
            
            BrushChanged.Invoke(null, new BrushChangedEventArgs(Cursor));
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

            if (brushColor == ColorsEnum.Eraser)
            {
                var path = $"{CursorsPath}/eraser-{Thickness}.png";
                Cursor = new(new Bitmap(assetLoader.Open(new Uri(path))), new PixelPoint((int)HalfThickness, (int)HalfThickness));
            }
            BrushChanged.Invoke(null, new BrushChangedEventArgs(Cursor));
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

public class BrushChangedEventArgs : EventArgs
{
    public BrushChangedEventArgs(Cursor cursor)
    {
        Cursor = cursor;
    }

    public Cursor Cursor { get; private set; }
}
