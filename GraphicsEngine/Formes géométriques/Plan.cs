using System.Numerics;

public class Plan : IGeometricShape
{
    public Point3D Point { get; private set; }  // Un point appartenant au plan
    public Normal Normal { get; private set; }  // Normale au plan (indique l'orientation)
    public Point3D Min { get; private set; }    // Coin minimal (optionnel pour un plan limité)
    public Point3D Max { get; private set; }    // Coin maximal (optionnel pour un plan limité)

    private RGBColor _diffuseColor = new RGBColor(1.0f, 0.0f, 1.0f); // Couleur par défaut : Violet

    public RGBColor DiffuseColor
    {
        get => _diffuseColor;
        set => _diffuseColor = value;
    }

    // Constructeur pour un plan infini


    // Constructeur pour un plan limité (rectangulaire)
    public Plan(Point3D point, Normal normal, Point3D min, Point3D max, RGBColor diffusecolor = null)
    {
        Min = min;
        Max = max;
        Normal = normal;
        Point = point;
        diffusecolor = DiffuseColor;
    }

    public bool Intersect(Ray ray, out float t)
    {
        t = 0.0f;

        // Boîte englobante
        BoundingBox boundingBox = CalculateBoundingBox();
        if(!boundingBox.Intersect(ray, 0.0, float.PositiveInfinity))
        {
            return false; // Le rayon ne touche pas la boîte
        }

        // Normale du plan
        Vector3 normal = new Vector3(Normal.X, Normal.Y, Normal.Z);

        // Direction du rayon
        Vector3 direction = new Vector3(ray.Direction.X, ray.Direction.Y, ray.Direction.Z);

        // Origine du rayon
        Vector3 origin = new Vector3(ray.Origin.X, ray.Origin.Y, ray.Origin.Z);

        // Produit scalaire entre la direction du rayon et la normale du plan
        float denom = Vector3.Dot(direction, normal);

        // Si le produit scalaire est proche de zéro, le rayon est parallèle au plan (pas d'intersection)
        if (MathF.Abs(denom) < 1e-6)
        {
            return false;
        }

        // Calcul de d selon la formule d = - (p ⋅ n)
        Vector3 point = new Vector3(Point.X, Point.Y, Point.Z); // Point sur le plan
        float d = -Vector3.Dot(point, normal);  // La constante d de l'équation du plan (remarque le signe négatif)

        // Calcul de t selon la formule t = -(d + o ⋅ n) / (d ⋅ n)
        t = -(Vector3.Dot(origin, normal) + d) / denom;

        // Si t est négatif, l'intersection est derrière l'origine du rayon (pas visible)
        if (t < 0)
        {
            return false;
        }

        // Calculer le point d'intersection
        Point3D intersectionPoint = ray.Call(t);

        // Vérifier si l'intersection est dans les limites du plan fini
        if (Min != null && Max != null)
        {
            if (intersectionPoint.X < Min.X || intersectionPoint.X > Max.X ||
                intersectionPoint.Y < Min.Y || intersectionPoint.Y > Max.Y ||  // Limite sur Y ajoutée ici
                intersectionPoint.Z < Min.Z || intersectionPoint.Z > Max.Z)
            {
                return false;  // L'intersection est hors des limites du plan fini
            }
        }

        return true;  // Intersection valide
    }




    // Calculer la normale au point d'intersection (constante car la normale d'un plan ne change pas)
    public Normal CalculateNormal(Point3D P)
    {
        return Normal;  // La normale est constante partout sur le plan
    }

    // Calculer l'aire du plan fini (si applicable)
    public float? CalculateArea()
    {
        if (Min != null && Max != null)
        {
            float largeur = Math.Abs(Max.X - Min.X);
            float hauteur = Math.Abs(Max.Y - Min.Y);
            return largeur * hauteur;
        }
        return null;  // Aire non définie pour un plan infini
    }

    // Représentation sous forme de chaîne de caractères
    public override string ToString()
    {
        return Min != null && Max != null
            ? $"Plane(Point: {Point}, Normal: {Normal}, Min: {Min}, Max: {Max})"
            : $"Plane(Point: {Point}, Normal: {Normal})";
    }

    // Calcul de la boîte englobante
    public BoundingBox CalculateBoundingBox()
    {
        if(Min == null || Max == null)
        {
            return null;
        }

        return new BoundingBox(Min, Max);
    }

    public void ApplyTransformation(GeometricTransform transform)
    {
        // Appliquer la transformation au point du plan
        Point = transform.ApplyTransformationOnPoint(Point);

        // Appliquer la transformation 
        Normal = transform.ApplyTransformationOnNormal(Normal);
    }

    // Subdivisions de la forme
    public void Refine(List<IGeometricShape> aObj)
    {
        // On subdivise le plan en plusieurs triangles
        int subdivisionsX = 15; // Pour mieux voir les interactions ombres-lumière, on va avec 15
        int subdivisionsY = 15; // Dans le cas où on voudrait voir les détails, on irait avec 25, 30, 45 ou 50

        // Calcul des dimensions des subdivisions
        float width = Max.X - Min.X;
        float height = Max.Y - Min.Y;

        float stepX = width / subdivisionsX;
        float stepY = height / subdivisionsY;

        for (int i = 0; i < subdivisionsX; i++) 
        {
            for (int j = 0; j < subdivisionsY; j++)
            {
                // Calcul des coins du carré à subdiviser en triangles
                Point3D bottomLeft = new Point3D(Min.X + i * stepX, Min.Y + j * stepY, Min.Z);
                Point3D bottomRight = new Point3D(Min.X + (i + 1) * stepX, Min.Y + j * stepY, Min.Z);
                Point3D topLeft = new Point3D(Min.X + i * stepX, Min.Y + (j + 1) * stepY, Min.Z);
                Point3D topRight = new Point3D(Min.X + (i + 1) * stepX, Min.Y + (j + 1) * stepY, Min.Z);

                // Créer deux triangles à partir des coins du carré
                aObj.Add(new Triangle(bottomLeft, bottomRight, topLeft, _diffuseColor)); // Triangle 1
                aObj.Add(new Triangle(bottomRight, topRight, topLeft, _diffuseColor)); // Triangle 2
            }
        }
    }

    // Récupérer la boîte englobante de la forme
    public BoundingBox ObjectBound()
    {
        return CalculateBoundingBox();
    }
}
