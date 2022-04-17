using TeamSketch.Models;

namespace TeamSketch.Services;

public interface IBrushService
{
    ColorsEnum Color { get; set; }
    ThicknessEnum Thickness { get; set; }
}

public class BrushService : IBrushService
{
    public ColorsEnum Color { get; set; }
    public ThicknessEnum Thickness { get; set; }
}
