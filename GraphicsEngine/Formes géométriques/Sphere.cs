using System;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

public class Sphere : IGeometricShape
{
    public float Radius { get; set; }
    public Point3D Center { get; private set; }
    public float MinRadius { get; set; } = 0.1f; // Rayon minimum pour la subdivision
    public RGBColor AmbientColor { get; set; }

    
    
    // Couleur par défaut
    private RGBColor _diffuseColor = new RGBColor(1.0f, 0.0f, 0.0f); // Couleur par défaut : rouge

    public RGBColor DiffuseColor 
    {
        get => _diffuseColor;
        set => _diffuseColor = value;
    }

    // Classe Sphere - Définition des couleurs
    public Sphere(float radius = 1.0f, Point3D center = null, RGBColor diffusecolor = null)
    {
        Radius = radius;
        Center = center ?? new Point3D();
        diffusecolor = DiffuseColor;

        // Couleur ambiante par défaut (faible influence)
        AmbientColor = new RGBColor(0.1f, 0.1f, 0.1f);

    }


    public void SetCenter(Point3D center)
    {
        Center = center;
    }

    public bool Intersect(Ray ray, out float t)
    {
        // Boîte englobante
        BoundingBox boundingBox = CalculateBoundingBox();
        if (!boundingBox.Intersect(ray, 0.0, float.PositiveInfinity))
        {
            t = 0;
            return false; // Le rayon ne touche pas la boîte
        }

        Vector3 oc = new Vector3(ray.Origin.X - Center.X, ray.Origin.Y - Center.Y, ray.Origin.Z - Center.Z);
        Vector3 direction = new Vector3(ray.Direction.X, ray.Direction.Y, ray.Direction.Z);

        float a = Vector3.Dot(direction, direction);
        float b = 2.0f * Vector3.Dot(oc, direction);
        float c = Vector3.Dot(oc, oc) - Radius * Radius;

        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            t = 0;
            return false;  // Pas d'intersection
        }

        float t0 = (-b - MathF.Sqrt(discriminant)) / (2.0f * a);
        float t1 = (-b + MathF.Sqrt(discriminant)) / (2.0f * a);

        // Choisir la plus petite solution positive (la plus proche de la caméra)
        if (t0 >= 0)
        {
            t = t0;
        }
        else if (t1 >= 0)
        {
            t = t1;
        }
        else
        {
            t = 0;
            return false;  // Pas d'intersection valide
        }

        return true;  // Intersection trouvée
    }



    public Normal CalculateNormal(Point3D Pinter)
    {
        return new Normal(
            (Pinter.X - Center.X) / Radius,
            (Pinter.Y - Center.Y) / Radius,
            (Pinter.Z - Center.Z) / Radius
        );
    }

    // Calcul de la boîte englobante
    public BoundingBox CalculateBoundingBox()
    {
        // Définir les points min et max
        Point3D minPoint = new Point3D(Center.X - Radius, Center.Y - Radius, Center.Z- Radius);
        Point3D maxPoint = new Point3D(Center.X + Radius, Center.Y + Radius, Center.Z + Radius);

        // Retourner une nouvelle boîte englobante
        return new BoundingBox(minPoint, maxPoint);
    }

    // Application d'une translation sur la sphère
    public void ApplyTransformation(GeometricTransform transform)
    {
        Point3D transformedCenter = transform.ApplyTransformationOnPoint(Center);
        Center = transformedCenter;
    }

    // Calcul de l'aire
    public float CalculateArea()
    {
        return 4 * MathF.PI * MathF.Pow(Radius, 2);
    }

    // Subdivisions de la forme
    public void Refine(List<IGeometricShape> aObj)
    {
        // On se base sur le principe de faire des sous-sphères qui recouvriront la sphère originale
        if (Radius <=  MinRadius)
            return;

        float newRadius = Radius / 2;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for(int k = -1; k <= 1; k++)
                {
                    if (Math.Abs(i) + Math.Abs(j) +  Math.Abs(k) == 1) // On ne prend que les sommets
                    {
                        Point3D newCenter = new Point3D(
                            Center.X + i * newRadius,
                            Center.Y + j * newRadius,
                            Center.Z + k * newRadius
                            );
                        aObj.Add(new Sphere { Center = newCenter, Radius = newRadius, MinRadius = MinRadius }); // On ajoute les sous-sphères à la liste
                    }
                }
            }
        }     
    }

    // Récupérer la boîte englobante de la forme
    public BoundingBox ObjectBound()
    {
        return CalculateBoundingBox();
    }
}
