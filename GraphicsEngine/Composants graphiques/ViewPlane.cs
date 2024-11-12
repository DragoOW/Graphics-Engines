using System;

public class ViewPlane
{
    public int XRes { get; private set; }
    public int YRes { get; private set; }
    public float PixelSize { get; private set; }
    public float Gamma { get; private set; }
    public int NumSamples { get; private set; }

    public ViewPlane(int xRes = 400, int yRes = 400, float pixelSize = 1.0f, float gamma = 1.0f, int numSamples = 250)
    {
        XRes = xRes;
        YRes = yRes;
        PixelSize = pixelSize;
        Gamma = gamma;
        NumSamples = numSamples;
    }

    public ViewPlane Copy()
    {
        return new ViewPlane(XRes, YRes, PixelSize, Gamma, NumSamples);
    }

    public override bool Equals(object obj)
    {
        if (obj is ViewPlane other)
        {
            return XRes == other.XRes &&
                   YRes == other.YRes &&
                   PixelSize == other.PixelSize &&
                   Gamma == other.Gamma && NumSamples == other.NumSamples;

        }
        return false;
    }

    public override int GetHashCode()
    {
        return XRes.GetHashCode() ^ YRes.GetHashCode() ^ PixelSize.GetHashCode() ^ Gamma.GetHashCode() ^ NumSamples.GetHashCode();
    }

    public void SetXRes(int xRes)
    {
        XRes = xRes;
    }

    public void SetYRes(int yRes)
    {
        YRes = yRes;
    }

    public void SetPixelSize(float pixelSize)
    {
        PixelSize = pixelSize;
    }

    public void SetGamma(float gamma)
    {
        Gamma = gamma;
    }

    public void SetNumSamples(int numSamples)
    {
        NumSamples = numSamples;
    }

    public override string ToString()
    {
        return $"ViewPlane(XRes: {XRes}, YRes: {YRes}, PixelSize: {PixelSize}, Gamma: {Gamma}, NumSamples: {NumSamples})";
    }
}
