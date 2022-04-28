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
}
