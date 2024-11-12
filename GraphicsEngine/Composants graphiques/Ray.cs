using System;

public class Ray
{
    public Point3D Origin { get; set; }
    public Normal Direction { get; set; }
    public int Depth { get; set; }
    public float? MinT { get; set; }
    public float? MaxT { get; set; }

    public Ray(Point3D origin = null, Normal direction = null, int depth = 0, float? mint = null, float? maxt = null)
    {
        Origin = origin ?? new Point3D();
        Direction = direction ?? new Normal();
        Depth = depth;
        MinT = mint;
        MaxT = maxt;
    }

    public override bool Equals(object obj)
    {
        if (obj is Ray other)
        {
            return Origin.Equals(other.Origin) && Direction.Equals(other.Direction) && Depth == other.Depth && MinT == other.MinT && MaxT == other.MaxT;
        }
        return false;
    }

    public override string ToString()
    {
        return $"Ray(Origin: {Origin}, Direction: {Direction}, Depth: {Depth}, MinT: {MinT}, MaxT: {MaxT})";
    }

    public void Assign(Ray other)
    {
        Origin = new Point3D(other.Origin.X, other.Origin.Y, other.Origin.Z);
        Direction = new Normal(other.Direction.X, other.Direction.Y, other.Direction.Z);
        Depth = other.Depth;
        MinT = other.MinT;
        MaxT = other.MaxT;
    }

    public Point3D Call(float t)
    {
        return new Point3D(
            Origin.X + t * Direction.X,
            Origin.Y + t * Direction.Y,
            Origin.Z + t * Direction.Z
        );
    }

    public override int GetHashCode()
    {
        return Origin.GetHashCode() ^ Direction.GetHashCode() ^ Depth.GetHashCode() ^ MinT.GetHashCode() ^ MaxT.GetHashCode();
    }
}
