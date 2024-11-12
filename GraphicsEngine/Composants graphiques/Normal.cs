using System;
using System.Numerics;

public class Normal
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Normal(float x = 0.0f, float y = 0.0f, float z = 0.0f)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public float Norm()
    {
        return MathF.Sqrt(SquareNorm());
    }

    public float SquareNorm()
    {
        return X * X + Y * Y + Z * Z;
    }

    public float Dot(Normal other)
    {
        return X * other.X + Y * other.Y + Z * other.Z;
    }

    public Normal Normalize()
    {
        float norm = Norm();
        if (norm == 0)
            throw new InvalidOperationException("Cannot normalize a zero vector.");
        return new Normal(X / norm, Y / norm, Z / norm);
    }

    public override bool Equals(object obj)
    {
        if (obj is Normal other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }
        return false;
    }

    public override string ToString()
    {
        return $"Normal({X}, {Y}, {Z})";
    }

    public void AssignFromVector(Vector3 vector)
    {
        X = vector.X;
        Y = vector.Y;
        Z = vector.Z;
    }

    public void AssignFromPoint(Point3D point)
    {
        X = point.X;
        Y = point.Y;
        Z = point.Z;
    }

    public void Assign(Normal other)
    {
        X = other.X;
        Y = other.Y;
        Z = other.Z;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
    }

    public Vector3 ToVector3()
    {
        return new Vector3(this.X, this.Y, this.Z);
    }
}
