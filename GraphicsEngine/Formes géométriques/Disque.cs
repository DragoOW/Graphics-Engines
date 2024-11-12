using System;
using System.Numerics;

public class Disque : IGeometricShape
{
    public Point3D Center { get; private set; }      // Centre du disque
    public float InnerRadius { get; private set; }   // Rayon intérieur
    public float OuterRadius { get; private set; }   // Rayon extérieur

    // Couleur par défaut
    private RGBColor _diffuseColor = new RGBColor(1.0f, 1.0f, 1.0f); // Couleur par défaut : blanc

    public RGBColor DiffuseColor
    {
        get => _diffuseColor;
        set => _diffuseColor = value;
    }

    // Constructeur du disque avec rayon intérieur, extérieur
    public Disque(Point3D center, float innerRadius, float outerRadius, RGBColor diffusecolor = null)
    {
        Center = center;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        diffusecolor = DiffuseColor;
    }


    // Méthode pour tester l'intersection entre un rayon et le disque
    public bool Intersect(Ray ray, out float t)
    {
        t = 0.0f;

        // Boîte englobante
        BoundingBox boundingBox = CalculateBoundingBox();

        // Vérifier que le rayon intersecte la boîte
        if (!boundingBox.Intersect(ray, 0.0, float.PositiveInfinity))
        {
            return false; // Le rayon ne touche pas la boîte
        }

        // Normale pointant dans la direction de l'axe Z (disque perpendiculaire à Z)
        Vector3 normal = new Vector3(0, 0, 1);
        float denom = Vector3.Dot(normal, ray.Direction.ToVector3());

        // Si le dénominateur est proche de 0, alors le rayon est parallèle au disque
        if (Math.Abs(denom) < float.Epsilon)
            return false;

        // Calculer la distance d'intersection avec le plan du disque (plan xy)
        Vector3 toCenter = new Vector3(Center.X - ray.Origin.X, Center.Y - ray.Origin.Y, Center.Z - ray.Origin.Z);
        t = Vector3.Dot(toCenter, normal) / denom;

        if (t < 0.0f)
            return false;  // Le disque est derrière la caméra

        // Calculer le point d'intersection sur le plan
        Point3D P = ray.Call(t);

        // Calculer la distance au centre du disque
        float distanceSquared = (P.X - Center.X) * (P.X - Center.X) + (P.Y - Center.Y) * (P.Y - Center.Y);

        // Vérifier si l'intersection est entre le rayon intérieur et extérieur
        if (distanceSquared < InnerRadius * InnerRadius || distanceSquared > OuterRadius * OuterRadius)
            return false;  // Le point est hors des limites du disque

        return true;  // Intersection trouvée
    }

    // Méthode pour retourner la normale du disque (elle est constante sur tout le disque)
    public Normal CalculateNormal(Point3D P)
    {
        return new Normal(0, 0, 1);  // Normale constante vers l'axe Z
    }

    // Calcul de la boîte englobante
    public BoundingBox CalculateBoundingBox()
    {
        // Définir le point min
        Point3D minPoint = new Point3D(
            Center.X - OuterRadius,
            Center.Y - OuterRadius,
            Center.Z
        );

        // Définir le point max
        Point3D maxPoint = new Point3D(
            Center.X + OuterRadius,
            Center.Y + OuterRadius,
            Center.Z
        );

        // Retourner une boîte englobante
        return new BoundingBox(minPoint, maxPoint);
    }

    // Calcul de l'aire
    public float CalculateArea()
    {
        return (MathF.PI * (MathF.Pow(InnerRadius, 2) - MathF.Pow(OuterRadius, 2)));
    }

    public void ApplyTransformation(GeometricTransform transform)
    {
        Point3D transformedCenter = transform.ApplyTransformationOnPoint(Center);
        Center = transformedCenter;
    }

    // Subdivisions de la forme
    public void Refine(List<IGeometricShape> aObj)
    {
        // On divise le disque en quadrants
        const int numSubdivisions = 4;
        float angleStep = MathF.PI / 2; //  Quadrants formées par des angles de 90 degrés

        for (int i = 0; i < numSubdivisions; i++)
        {
            float angle1 = i * angleStep;
            float angle2 = (i + 1) * angleStep;

            // Calcul des coins extérieurs
            Point3D outerPoint1 = new Point3D(
                Center.X + OuterRadius * MathF.Cos(angle1),
                Center.Y + OuterRadius * MathF.Sin(angle1),
                Center.Z
            );

            Point3D outerPoint2 = new Point3D(
                Center.X + OuterRadius * MathF.Cos(angle2),
                Center.Y + OuterRadius * MathF.Sin(angle2),
                Center.Z
            );

            // Calcul des coins intérieurs
            Point3D innerPoint1 = new Point3D(
                Center.X + InnerRadius * MathF.Cos(angle1),
                Center.Y + InnerRadius * MathF.Sin(angle1),
                Center.Z
            );

            Point3D innerPoint2 = new Point3D(
                Center.X + InnerRadius * MathF.Cos(angle2),
                Center.Y + InnerRadius * MathF.Sin(angle2),
                Center.Z
            );

            // Créer les 4 triangles pour le quadrant
            /* Chaque quadrant est formé de deux triangles, un qui relie les points intérieurs et extérieurs 
             * puis un qui complète le quadrant avec le point intérieur suivant */
            aObj.Add(new Triangle(innerPoint1, outerPoint1, outerPoint2, _diffuseColor));
            aObj.Add(new Triangle(innerPoint1, outerPoint2, innerPoint2, _diffuseColor));
        }
    }

    // Récupérer la boîte englobante de la forme
    public BoundingBox ObjectBound()
    {
        return CalculateBoundingBox();
    }
}
