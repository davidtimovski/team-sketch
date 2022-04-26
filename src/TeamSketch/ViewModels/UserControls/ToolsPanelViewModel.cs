using ReactiveUI;
using TeamSketch.Models;
using TeamSketch.Utils;

namespace TeamSketch.ViewModels.UserControls;

public class ToolsPanelViewModel : ViewModelBase
{
    private ColorsEnum brushColor = BrushSettings.BrushColor;
    private ColorsEnum BrushColor
    {
        get => brushColor;
        set
        {
            this.RaiseAndSetIfChanged(ref brushColor, value);
            BrushSettings.BrushColor = value;

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

    private ThicknessEnum previousBrushThickness = BrushSettings.BrushThickness;
    private ThicknessEnum brushThickness = BrushSettings.BrushThickness;
    private ThicknessEnum BrushThickness
    {
        get => brushThickness;
        set
        {
            this.RaiseAndSetIfChanged(ref brushThickness, value);
            BrushSettings.BrushThickness = value;

            if (brushColor != ColorsEnum.Eraser || value != ThicknessEnum.Eraser)
            {
                previousBrushThickness = brushThickness;
            }
        }
    }
}
