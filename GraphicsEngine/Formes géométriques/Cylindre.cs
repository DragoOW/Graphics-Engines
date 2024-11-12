using System;
using System.Numerics;

public class Cylinder : IGeometricShape
{
    public Point3D BaseCenter { get; private set; } // Centre de la base du cylindre
    public float Height { get; private set; }       // Hauteur du cylindre
    public float Radius { get; private set; }       // Rayon du cylindre

    // Couleur par défaut
    private RGBColor _diffuseColor = new RGBColor(1.0f, 1.0f, 1.0f); // Couleur par défaut : blanc

    public RGBColor DiffuseColor
    {
        get => _diffuseColor;
        set => _diffuseColor = value;
    }
    // Constructeur du cylindre avec le centre de la base, la hauteur et le rayon
    public Cylinder(Point3D baseCenter, float height, float radius, RGBColor diffusecolor = null)
    {
        BaseCenter = baseCenter;
        Height = height;
        Radius = radius;
        diffusecolor = DiffuseColor;
    }

    // Méthode pour tester l'intersection avec un rayon
    public bool Intersect(Ray ray, out float t)
    {
        t = float.PositiveInfinity;

        // Boîte englobante
        BoundingBox boundingBox = CalculateBoundingBox();
        if(!boundingBox.Intersect(ray, 0.0, float.PositiveInfinity))
        {
            return false; // Le rayon ne touche pas la boîte
        }

        // Convertir les données de la base du cylindre et du rayon en Vector3
        Vector3 baseCenterVec = BaseCenter.ToVector3();
        Vector3 rayOrigin = ray.Origin.ToVector3();
        Vector3 rayDirection = ray.Direction.ToVector3();

        // Définir l'axe du cylindre (aligné sur l'axe Y dans cet exemple)
        Vector3 axis = new Vector3(0, 1, 0); // L'axe du cylindre est vertical

        // Projection du rayon dans le plan XZ (orthogonal à l'axe du cylindre)
        Vector3 deltaP = rayOrigin - baseCenterVec;
        Vector3 dXZ = new Vector3(rayDirection.X, 0, rayDirection.Z);
        Vector3 pXZ = new Vector3(deltaP.X, 0, deltaP.Z);

        // Résoudre l'équation quadratique pour l'intersection dans le plan XZ
        float a = Vector3.Dot(dXZ, dXZ);
        float b = 2 * Vector3.Dot(pXZ, dXZ);
        float c = Vector3.Dot(pXZ, pXZ) - Radius * Radius;

        // Calcul du discriminant
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0.0f)
            return false; // Pas d'intersection si le discriminant est négatif

        float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
        float t0 = (-b - sqrtDiscriminant) / (2 * a);
        float t1 = (-b + sqrtDiscriminant) / (2 * a);

        // Choisir la plus petite valeur positive de t
        if (t0 > 0.0f)
            t = t0;
        else if (t1 > 0.0f)
            t = t1;
        else
            return false;

        // Calculer l'intersection le long de l'axe Y (hauteur du cylindre)
        float yIntersect = rayOrigin.Y + t * rayDirection.Y;
        if (yIntersect < BaseCenter.Y || yIntersect > BaseCenter.Y + Height)
            return false; // Si l'intersection est en dehors de la hauteur du cylindre

        return true;
    }

    // Méthode pour calculer la normale au point d'intersection
    public Normal CalculateNormal(Point3D P)
    {
        // Calculer la normale à la surface du cylindre
        Vector3 point = P.ToVector3();
        Vector3 baseCenterVec = BaseCenter.ToVector3();

        // Projeter le point sur le plan XZ pour obtenir la normale
        Vector3 normalVec = new Vector3(point.X - baseCenterVec.X, 0, point.Z - baseCenterVec.Z);
        normalVec = Vector3.Normalize(normalVec);

        return new Normal(normalVec.X, normalVec.Y, normalVec.Z);
    }

    // Calcul de la boîte englobante
    public BoundingBox CalculateBoundingBox()
    {
        if(BaseCenter == null)
        {
            throw new InvalidOperationException("BaseCenter must be defined.");
        }

        // Calculer les coins de la boîte englobante
        Point3D min = new Point3D(
            BaseCenter.X - Radius, // x_min
            BaseCenter.Y,          // y_min (base du cylindre)
            BaseCenter.Z - Radius  // z_min
        );

        Point3D max = new Point3D(
            BaseCenter.X + Radius,  // x_max
            BaseCenter.Y + Height,  // y_max (sommet du cylindre)
            BaseCenter.Z + Radius   // z_max
        );

        return new BoundingBox(min, max);
    }

    // Calcul de l'aire
    public float CalculateArea()
    {
        return 2 * MathF.PI * Radius * ((BaseCenter.Z + Radius) - (BaseCenter.Y));
    }

    public void ApplyTransformation(GeometricTransform transform)
    {
        throw new NotImplementedException();
    }

    // Subdivisions de la forme
    public void Refine(List<IGeometricShape> aObj)
    {
        // Diviser le cylindre en deux plus petits cylindres
        float halfHeight = Height / 2;

        var newCylinders = new List<Cylinder>
        {
            new Cylinder (BaseCenter = BaseCenter, Height = halfHeight, Radius = Radius),
            new Cylinder (BaseCenter = new Point3D(BaseCenter.X, BaseCenter.Y, BaseCenter.Z + halfHeight), Height = halfHeight, Radius = Radius)
        };

        foreach (var newCylinder in newCylinders)
        {
            aObj.Add(newCylinder); // On ajoute les deux cylindres crées dans la liste
        }
    }

    // Récupérer la boîte englobante de la forme
    public BoundingBox ObjectBound()
    {
        return CalculateBoundingBox();
    }
}
