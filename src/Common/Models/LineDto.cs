namespace Common
{
    public record LineDto(double X1, double Y1, double X2, double Y2, double thickness, ColorsEnum color) : ShapeDto(color);
}
