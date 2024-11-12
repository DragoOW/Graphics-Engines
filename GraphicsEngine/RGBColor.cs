using System.Drawing;

public class RGBColor
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }

    public RGBColor(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public Color ToColor()
    {
        return Color.FromArgb((int)(R * 255), (int)(G * 255), (int)(B * 255));
    }

    public static RGBColor operator +(RGBColor c1, RGBColor c2)
    {
        return new RGBColor(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
    }

    public static RGBColor operator *(float intensity, RGBColor c)
    {
        return new RGBColor(c.R * intensity, c.G * intensity, c.B * intensity);
    }

    public static RGBColor operator /(RGBColor c, int value)
    {
        if (value == 0)
        {
            throw new DivideByZeroException("Attempted to divide by zero in RGBColor.");
        }

        return new RGBColor(c.R / value, c.G / value, c.B / value);
    }
}
