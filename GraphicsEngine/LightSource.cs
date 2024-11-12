public class LightSource
{
    public Point3D Position { get; private set; }
    public RGBColor Color { get; private set; }
    public float Intensity { get; private set; }

    public LightSource(Point3D position, RGBColor color, float intensity)
    {
        Position = position;
        Color = color;
        Intensity = intensity;
    }
}
