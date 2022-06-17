namespace TeamSketch;

public static class Globals
{
    public const int CanvasWidth = 1280;
    public const int CanvasHeight = 720;

#if DEBUG
    public const string ServerUri = "http://localhost:5150";
#else
    public const string ServerUri = "https://team-sketch.davidtimovski.com";
#endif

    private static short renderingIntervalMs = 10;
    public static short RenderingIntervalMs
    {
        get
        {
            return renderingIntervalMs;
        }
        set
        {
            if (value > 1000)
            {
                renderingIntervalMs = 1000;
            }
            else if (value < 3)
            {
                renderingIntervalMs = 3;
            }
            else
            {
                renderingIntervalMs = value;
            }
        }
    }
}
