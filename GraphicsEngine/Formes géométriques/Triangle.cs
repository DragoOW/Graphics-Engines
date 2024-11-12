using System;
using System.Numerics; // Pour les calculs vectoriels

public class Triangle : IGeometricShape
{
    public Point3D V0 { get; private set; } // Premier sommet du triangle
    public Point3D V1 { get; private set; } // Deuxième sommet du triangle
    public Point3D V2 { get; private set; } // Troisième sommet du triangle
  
    // Couleur par défaut
    private RGBColor _diffuseColor = new RGBColor(1.0f, 1.0f, 1.0f); // Couleur par défaut : blanc

    public RGBColor DiffuseColor 
    { 
        get => _diffuseColor;
        set => _diffuseColor = value;
    }

    // Constructeur du triangle avec les trois sommets
    public Triangle(Point3D v0, Point3D v1, Point3D v2, RGBColor diffusecolor = null)
    {
        V0 = v0;
        V1 = v1;
        V2 = v2;
        diffusecolor = DiffuseColor;
        
    }

    // Méthode d'intersection avec un rayon (algorithme de Möller–Trumbore)
    public bool Intersect(Ray ray, out float t)
    {
        t = float.PositiveInfinity;

        // Boîte englobante
        BoundingBox boundingBox = CalculateBoundingBox();
        if(!boundingBox.Intersect(ray, 0.0, float.PositiveInfinity))
        {
            return false; // Le rayon ne touche pas la boîte englobante
        }

        // Vérifier si le rayon est parallèle au triangle
        Vector3 v0 = V0.ToVector3();
        Vector3 v1 = V1.ToVector3();
        Vector3 v2 = V2.ToVector3();

        Vector3 edge1 = v1 - v0; // v1 - v0
        Vector3 edge2 = v2 - v0; // v2 - v0
        Vector3 pvec = Vector3.Cross(ray.Direction.ToVector3(), edge2); // pvec = d x edge2
        float det = Vector3.Dot(edge1, pvec); // det = edge1 · pvec

        // Si le déterminant est proche de zéro, le rayon est parallèle au triangle
        if (Math.Abs(det) < float.Epsilon)
            return false;

        float invDet = 1.0f / det;

        // Calculer les barycentriques (u, v)
        Vector3 tvec = ray.Origin.ToVector3() - v0; // tvec = o - v0
        float u = Vector3.Dot(tvec, pvec) * invDet; // u = tvec · pvec * invDet
        if (u < 0.0f || u > 1.0f)
            return false;

        Vector3 qvec = Vector3.Cross(tvec, edge1); // qvec = tvec x edge1
        float v = Vector3.Dot(ray.Direction.ToVector3(), qvec) * invDet; // v = d · qvec * invDet
        if (v < 0.0f || u + v > 1.0f)
            return false;

        // Calculer t (la distance d'intersection)
        t = Vector3.Dot(edge2, qvec) * invDet;

        // Si t est positif, alors le rayon intersecte le triangle
        return t > 0.0f;
    }

    // Calcul de la normale au triangle
    public Normal CalculateNormal(Point3D p)
    {
        Vector3 edge1 = V1.ToVector3() - V0.ToVector3();
        Vector3 edge2 = V2.ToVector3() - V0.ToVector3();
        Vector3 normal = Vector3.Cross(edge1, edge2);
        Vector3 normalizedVector = Vector3.Normalize(normal);
        return new Normal(normalizedVector.X, normalizedVector.Y, normalizedVector.Z);
    }

    // Volume englobant du triangle
    public BoundingBox CalculateBoundingBox()
    {
        // Initialiser les points min et max avec des valeurs infinies
        BoundingBox TriangleBox = new BoundingBox();

        // Mettre à jour les points min et max en fonction des sommets du triangle
        TriangleBox.m_pMin.X = Math.Min(TriangleBox.m_pMin.X, Math.Min(V0.X, Math.Min(V1.X, V2.X)));
        TriangleBox.m_pMin.Y = Math.Min(TriangleBox.m_pMin.Y, Math.Min(V0.Y, Math.Min(V1.Y, V2.Y)));
        TriangleBox.m_pMin.Z = Math.Min(TriangleBox.m_pMin.Z, Math.Min(V0.Z, Math.Min(V1.Z, V2.Z)));

        TriangleBox.m_pMax.X = Math.Max(TriangleBox.m_pMax.X, Math.Max(V0.X, Math.Max(V1.X, V2.X)));
        TriangleBox.m_pMax.Y = Math.Max(TriangleBox.m_pMax.Y, Math.Max(V0.Y, Math.Max(V1.Y, V2.Y)));
        TriangleBox.m_pMax.Z = Math.Max(TriangleBox.m_pMax.Z, Math.Max(V0.Z, Math.Max(V1.Z, V2.Z)));

        // Retourner la boîte englobante
        return new BoundingBox(TriangleBox.m_pMin, TriangleBox.m_pMax);
    }

    // Calcul de l'aire
    public float CalculateArea()
    {
        Vector3 edge1 = V1.ToVector3() - V0.ToVector3();
        Vector3 edge2 = V2.ToVector3() - V0.ToVector3();
        return 0.5f * Vector3.Cross(edge1, edge2).Length(); // Aire = 1/2 * || (p2 - p1) X (p3 - p1) ||
    }

    public void ApplyTransformation(GeometricTransform transform)
    {
        V0 = transform.ApplyTransformationOnPoint(V0);
        V1 = transform.ApplyTransformationOnPoint(V1);
        V2 = transform.ApplyTransformationOnPoint(V2);
    }

    // Subdivisions de la forme
    public void Refine(List<IGeometricShape> aObj)
    {
        // Diviser le triangle en 4 sous-triangles
        Point3D midAB = Point3D.MidPoint(V0, V1);
        Point3D midBC = Point3D.MidPoint(V1, V2);
        Point3D midCA = Point3D.MidPoint(V2, V0);

        // Ajout des sous-triangles dans la liste
        aObj.Add(new Triangle (V0 = V0, V1 = midAB, V2 = midCA));
        aObj.Add(new Triangle (V0 = V1, V1 = midBC, V2 = midAB));
        aObj.Add(new Triangle (V0 = V2, V1 = midCA, V2 = midBC));
        aObj.Add(new Triangle (V0 = midAB, V1 = midBC, V2 = midCA));
    }

    // Récupérer la boîte englobante de la forme
    public BoundingBox ObjectBound()
    {
        return CalculateBoundingBox();
    }
}
