
public interface IGeometricShape
{
    public RGBColor DiffuseColor { get; set; }

    public void ApplyTransformation(GeometricTransform transform);

    public bool Intersect(Ray ray, out float t);

    public Normal CalculateNormal(Point3D p);

    void Refine(List<IGeometricShape> aObj);

    BoundingBox ObjectBound(); 
}

