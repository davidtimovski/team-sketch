namespace Common
{
    public record PointDto(double X, double Y, double size, ColorsEnum color) : ShapeDto(color);
}
