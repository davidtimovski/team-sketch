using ReactiveUI;
using TeamSketch.Models;

namespace TeamSketch.ViewModels.UserControls;

public sealed class ToolsPanelViewModel : ReactiveObject
{
    private readonly BrushSettings _brushSettings;

    public ToolsPanelViewModel(BrushSettings brushSettings)
    {
        _brushSettings = brushSettings;

        brushColor = _brushSettings.BrushColor;
        previousBrushThickness = _brushSettings.BrushThickness;
        brushThickness = _brushSettings.BrushThickness;
    }

    private ColorsEnum brushColor;
    public ColorsEnum BrushColor
    {
        get => brushColor;
        set
        {
            this.RaiseAndSetIfChanged(ref brushColor, value);
            _brushSettings.BrushColor = value;

            if (value == ColorsEnum.Eraser)
            {
                BrushThickness = ThicknessEnum.Eraser;
            }
            else
            {
                BrushThickness = previousBrushThickness;
            }
        }
    }

    private ThicknessEnum previousBrushThickness;
    private ThicknessEnum brushThickness;
    public ThicknessEnum BrushThickness
    {
        get => brushThickness;
        set
        {
            this.RaiseAndSetIfChanged(ref brushThickness, value);
            _brushSettings.BrushThickness = value;

            if (brushColor != ColorsEnum.Eraser || value != ThicknessEnum.Eraser)
            {
                previousBrushThickness = brushThickness;
            }
        }
    }
}
