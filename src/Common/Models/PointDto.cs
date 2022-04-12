namespace Common
{
    public record PointDto(double X, double Y, double Size, ColorsEnum Color) : ShapeDto(Color);
}
