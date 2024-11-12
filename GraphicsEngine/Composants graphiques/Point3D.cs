
using System.Numerics;

public class Point3D
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Point3D(float x = 0.0f, float y = 0.0f, float z = 0.0f)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Point3D operator -(Point3D p)
    {
        return new Point3D(-p.X, -p.Y, -p.Z);
    }

    public static Point3D operator -(Point3D p1, Point3D p2)
    {
        return new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
    }

    public static Point3D operator +(Point3D p1, Point3D p2)
    {
        return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
    }

    public static Point3D operator *(Point3D p, float scalar)
    {
        return new Point3D(p.X * scalar, p.Y * scalar, p.Z * scalar);
    }

    public static Point3D operator /(Point3D p, float scalar)
    {
        if (scalar == 0)
            throw new DivideByZeroException("Cannot divide by zero.");
        return new Point3D(p.X / scalar, p.Y / scalar, p.Z / scalar);
    }

    public static bool operator ==(Point3D p1, Point3D p2)
    {
        // Si les deux sont nulls, ils sont égaux
        if (ReferenceEquals(p1, null) && ReferenceEquals(p2, null))
        {
            return true;
        }

        // Si l'un est null et l'autre non, ils sont différents
        if (ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
        {
            return false;
        }

        // Sinon, comparer les valeurs des coordonnées
        return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
    }

    public static bool operator !=(Point3D p1, Point3D p2)
    {
        return !(p1 == p2);
    }

    public float this[int index]
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException("Invalid index.")
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                case 2:
                    Z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid index.");
            }
        }
    }

    public static float Distance(Point3D p1, Point3D p2)
    {
        float dx = p1.X - p2.X;
        float dy = p1.Y - p2.Y;
        float dz = p1.Z - p2.Z;
        return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public override bool Equals(object obj)
    {
        if (obj is Point3D point)
        {
            return this == point;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
    }

    public override string ToString()
    {
        return $"Point3D({X}, {Y}, {Z})";
    }
    public Vector3 ToVector3()
    {
        return new Vector3(this.X, this.Y, this.Z);
    }

    public static Point3D MidPoint(Point3D a, Point3D b)
    {
        return new Point3D(
            (a.X + b.X) / 2,
            (a.Y + b.Y) / 2,
            (a.Z + b.Z) / 2
        );
    }
}
