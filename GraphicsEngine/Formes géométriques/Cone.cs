
using System;
using System.Numerics;

public class Cone : IGeometricShape
{
    public Point3D Apex { get; private set; } // Le sommet du cône
    public float Height { get; private set; } // La hauteur du cône
    public float Radius { get; private set; } // Le rayon de la base

    // Couleur par défaut
    private RGBColor _diffuseColor = new RGBColor(1.0f, 1.0f, 1.0f); // Couleur par défaut : blanc

    public RGBColor DiffuseColor 
    { 
        get => _diffuseColor;
        set => _diffuseColor = value;
    }

    // Constructeur du cône avec sommet, hauteur et rayon
    public Cone(Point3D apex, float height, float radius, RGBColor diffusecolor = null)
    {
        Apex = apex;
        Height = height;
        Radius = radius;
        diffusecolor = DiffuseColor;
        
    }

    // Méthode pour tester l'intersection entre un rayon et le cône
    public bool Intersect(Ray ray, out float t)
    {
        t = float.PositiveInfinity;

        // Boîte englobante
        BoundingBox boundingBox = CalculateBoundingBox();

        // Vérifier que le rayon intersecte la boîte englobante
        if (!boundingBox.Intersect(ray, 0.0, float.PositiveInfinity))
        {
            return false; // Pas d'intersection si le rayon ne touche pas la boîte englobante
        }

        // Coefficients de l'équation quadratique pour un cône
        Vector3 vApex = Apex.ToVector3();
        Vector3 rayOrigin = ray.Origin.ToVector3();
        Vector3 rayDirection = ray.Direction.ToVector3();

        Vector3 v = rayOrigin - vApex; // Vector from apex to ray origin
        float k = (Radius / Height) * (Radius / Height); // Rapport entre le rayon et la hauteur

        float a = rayDirection.X * rayDirection.X + rayDirection.Z * rayDirection.Z - k * rayDirection.Y * rayDirection.Y;
        float b = 2.0f * (rayDirection.X * v.X + rayDirection.Z * v.Z - k * rayDirection.Y * v.Y);
        float c = v.X * v.X + v.Z * v.Z - k * v.Y * v.Y;

        // Calcul du discriminant pour l'équation quadratique
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0.0f)
            return false; // Pas d'intersection si le discriminant est négatif

        // Calcul de t1 et t2 (les distances d'intersection)
        float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
        float t0 = (-b - sqrtDiscriminant) / (2 * a);
        float t1 = (-b + sqrtDiscriminant) / (2 * a);

        // Garder le plus petit t positif
        if (t0 > 0.0f)
            t = t0;
        else if (t1 > 0.0f)
            t = t1;
        else
            return false;

        // Calcul de l'intersection au niveau de la base (limite supérieure du cône)
        Vector3 P = rayOrigin + t * rayDirection;
        float yIntersect = Apex.Y - P.Y;

        if (yIntersect < 0 || yIntersect > Height)
            return false; // Si l'intersection se trouve en dehors de la hauteur du cône

        return true;
    }

    // Calculer la normale au point d'intersection
    public Normal CalculateNormal(Point3D P)
    {
        Vector3 vApex = Apex.ToVector3();
        Vector3 p = P.ToVector3();

        // Normale à la surface du cône
        Vector3 normalVector = Vector3.Normalize(new Vector3(p.X - vApex.X, -(Radius / Height) * (p.Y - vApex.Y), p.Z - vApex.Z));
        return new Normal(normalVector.X, normalVector.Y, normalVector.Z);
    }

    // Calculer la boîte englobante du cône
    public BoundingBox CalculateBoundingBox()
    {
        // Définir le point min
        Point3D minPoint = new Point3D(
            Apex.X - Radius,
            Apex.Y - Height,
            Apex.Z - Radius
        );

        // Définir le point max
        Point3D maxPoint = new Point3D(
            Apex.X + Radius,
            Apex.Y,
            Apex.Z + Radius
        );

        // Retourner la boîte englobante
        return new BoundingBox(minPoint, maxPoint);
    }

    // Calcul de l'aire d'un cône
    public float CalculateArea()
    {
        double LateralArea = Math.PI * Radius * (Math.Sqrt(Math.Pow(Radius, 2.0) + Math.Pow(Height, 2.0)));
        double BaseArea = Math.PI * Math.Pow(Radius, 2.0);
        float TotalArea = (float)((float)LateralArea + BaseArea);

        return TotalArea;
    }

    public void ApplyTransformation(GeometricTransform transform)
    {
        // Appliquer la translation à l'apex
        if (transform != null && !transform.IsIdentity())
        {
            // Appliquer la transformation sur l'apex
            Apex = transform.ApplyTransformationOnPoint(Apex);

            // Appliquer la transformation sur la hauteur et le rayon
            // En cas de changement d'échelle
            Vector scale = new Vector(transform.m_mat.M11, transform.m_mat.M22, transform.m_mat.M33);
            Height *= scale.y; // On utilise m_y pour la hauteur
            Radius *= scale.x; // On utilise m_x pour le rayon

            // Calculer la rotation de l'apex
            Vector apexVector = new Vector(Apex.X, Apex.Y, Apex.Z);
            apexVector = transform.rotate(apexVector, transform.theta, new Vector(1, 0, 0)); // Exemple avec l'axe X
            Apex = new Point3D(apexVector.x, apexVector.y, apexVector.z);
        }

        // Recalculer la boîte englobante
        BoundingBox boundingBox = CalculateBoundingBox();
    }

    // Subdivisions de la forme
    public void Refine(List<IGeometricShape> aObj)
    {
        // On sépare le cône en plusieurs segments (tranches)
        int numSegments = 8;
        float angleStep = 2 * MathF.PI / numSegments;

        for (int i = 0; i < numSegments; i++)
        {
            float angle1 = i * angleStep;
            float angle2 = (i + 1) * angleStep;

            // Points de la base du cône
            Point3D basePoint1 = new Point3D(
                Apex.X + Radius * MathF.Cos(angle1),
                Apex.Y + Radius * MathF.Sin(angle1),
                Apex.Z - Height
            );

            Point3D basePoint2 = new Point3D(
                Apex.X + Radius * MathF.Cos(angle2),
                Apex.Y + Radius * MathF.Sin(angle2),
                Apex.Z - Height
            );

            // Triangle entre l'apex et les deux points de la base
            aObj.Add(new Triangle(Apex, basePoint1, basePoint2, _diffuseColor));
        }
    }

    // Récupérer la boîte englobante de la forme
    public BoundingBox ObjectBound()
    {
        return CalculateBoundingBox();
    }
}

