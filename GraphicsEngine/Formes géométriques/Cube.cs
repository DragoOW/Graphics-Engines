using System;
using System.Numerics;

public class Cube : IGeometricShape
{
    public Point3D Min { get; private set; }  // Coin minimal du cube (xmin, ymin, zmin)
    public Point3D Max { get; private set; }  // Coin maximal du cube (xmax, ymax, zmax)

    // Couleur par défaut
    private RGBColor _diffuseColor = new RGBColor(1.0f, 1.0f, 1.0f); // Couleur par défaut : blanc

    public RGBColor DiffuseColor
    {
        get => _diffuseColor;
        set => _diffuseColor = value;
    }

    // Niveau de subdivisions
    private int _subdivisionLevel;

    // Constructeur par défaut
    public Cube(Point3D min = null, Point3D max = null, RGBColor diffusecolor = null, int subdivisionLevel = 0)
    {
        Min = min ?? new Point3D(-1.0f, -1.0f, -1.0f);  // Coin minimal par défaut
        Max = max ?? new Point3D(1.0f, 1.0f, 1.0f);     // Coin maximal par défaut
        diffusecolor = DiffuseColor;
        _subdivisionLevel = subdivisionLevel;
    }

    // Méthode pour tester l'intersection entre un rayon et le cube
    public bool Intersect(Ray ray, out float tMin)
    {
        tMin = float.NegativeInfinity;
        float tMax = float.PositiveInfinity;

        // Boîte englobante
        BoundingBox boundingBox = CalculateBoundingBox();
        if (!boundingBox.Intersect(ray, float.NegativeInfinity, float.PositiveInfinity))
        {
            return false; // Le rayon ne touche pas la boîte
        }

        // Vérifier les 3 dimensions du cube (x, y, z)
        for (int i = 0; i < 3; i++)
        {
            float originComponent = i switch
            {
                0 => ray.Origin.X,
                1 => ray.Origin.Y,
                2 => ray.Origin.Z,
                _ => throw new IndexOutOfRangeException()
            };

            float directionComponent = i switch
            {
                0 => ray.Direction.X,
                1 => ray.Direction.Y,
                2 => ray.Direction.Z,
                _ => throw new IndexOutOfRangeException()
            };

            float minComponent = i switch
            {
                0 => Min.X,
                1 => Min.Y,
                2 => Min.Z,
                _ => throw new IndexOutOfRangeException()
            };

            float maxComponent = i switch
            {
                0 => Max.X,
                1 => Max.Y,
                2 => Max.Z,
                _ => throw new IndexOutOfRangeException()
            };

            float invD = 1.0f / directionComponent;
            float t0 = (minComponent - originComponent) * invD;
            float t1 = (maxComponent - originComponent) * invD;

            if (invD < 0)
            {
                // Échanger t0 et t1 si l'inverse de la direction est négatif
                float temp = t0;
                t0 = t1;
                t1 = temp;
            }

            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;

            if (tMax <= tMin)
            {
                return false;  // Pas d'intersection
            }
        }

        return true;  // Intersection trouvée
    }

    // Calculer la normale au point d'intersection
    public Normal CalculateNormal(Point3D P)
    {
        // Vérifier sur quelle face du cube se trouve le point d'intersection
        if (Math.Abs(P.X - Min.X) < 0.001f) return new Normal(-1.0f, 0.0f, 0.0f);
        if (Math.Abs(P.X - Max.X) < 0.001f) return new Normal(1.0f, 0.0f, 0.0f);
        if (Math.Abs(P.Y - Min.Y) < 0.001f) return new Normal(0.0f, -1.0f, 0.0f);
        if (Math.Abs(P.Y - Max.Y) < 0.001f) return new Normal(0.0f, 1.0f, 0.0f);
        if (Math.Abs(P.Z - Min.Z) < 0.001f) return new Normal(0.0f, 0.0f, -1.0f);
        if (Math.Abs(P.Z - Max.Z) < 0.001f) return new Normal(0.0f, 0.0f, 1.0f);

        return new Normal(0.0f, 0.0f, 0.0f);  // Si le point n'est pas sur une face, renvoyer une normale nulle
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

    // Calcul de l'aire
    public float CalculateArea()
    {
        if (Min == null || Max == null)
        {
            throw new InvalidOperationException("Min and Max must be defined.");
        }
        
        // Calculer la longueur d'un côté du cube
        float lengthX = Max.X - Min.X;
        float lengthY = Max.Y - Min.Y;
        float lengthZ = Max.Z - Min.Z;

        // L'aire d'une face du cube
        float areaFace = lengthX * lengthY;

        // Aire totale
        return 6 * areaFace;
    }

    public void ApplyTransformation(GeometricTransform transform)
    {
        // Vérifier si la transformation est valide
        if (transform != null && !transform.IsIdentity())
        {
            // Appliquer la transformation sur les coins du cube
            Point3D minTransformed = transform.ApplyTransformationOnPoint(Min);
            Point3D maxTransformed = transform.ApplyTransformationOnPoint(Max);

            // Mettre à jour les coins transformés
            Min = minTransformed;
            Max = maxTransformed;

            // Calculer l'échelle
            Vector scale = new Vector(transform.m_mat.M11, transform.m_mat.M22, transform.m_mat.M33);

            // Appliquer l'échelle aux dimensions du cube
            Vector3 center = new Vector3((Min.X + Max.X) / 2, (Min.Y + Max.Y) / 2, (Min.Z + Max.Z) / 2);
            Vector3 size = new Vector3(Max.X - Min.X, Max.Y - Min.Y, Max.Z - Min.Z);

            size *= new Vector3(scale.x, scale.y, scale.z); // Appliquer l'échelle

            // Mettre à jour les nouveaux coins après l'échelle
            Min = new Point3D(center.X - size.X / 2, center.Y - size.Y / 2, center.Z - size.Z / 2);
            Max = new Point3D(center.X + size.X / 2, center.Y + size.Y / 2, center.Z + size.Z / 2);

            // Rotation (exemple: autour de l'axe Y)
            // Vous pouvez appliquer une rotation selon les besoins
            // Pour cela, vous devrez également obtenir tous les coins du cube
            Point3D[] corners = GetCorners();
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = transform.ApplyTransformationOnPoint(corners[i]);
            }

            // Mettre à jour les Min et Max après la rotation
            Min = corners[0]; // Initialiser avec le premier coin
            Max = corners[0]; // Initialiser avec le premier coin

            // Mettre à jour Min et Max en fonction des coins transformés
            foreach (var corner in corners)
            {
                Min = new Point3D(Math.Min(Min.X, corner.X), Math.Min(Min.Y, corner.Y), Math.Min(Min.Z, corner.Z));
                Max = new Point3D(Math.Max(Max.X, corner.X), Math.Max(Max.Y, corner.Y), Math.Max(Max.Z, corner.Z));
            }
        }

        // Recalculer la boîte englobante (si nécessaire)
        BoundingBox boundingBox = CalculateBoundingBox();
    }

    // Méthode pour obtenir les coins du cube
    private Point3D[] GetCorners()
    {
        return new Point3D[]
        {
        Min,
        new Point3D(Max.X, Min.Y, Min.Z),
        new Point3D(Min.X, Max.Y, Min.Z),
        new Point3D(Max.X, Max.Y, Min.Z),
        new Point3D(Min.X, Min.Y, Max.Z),
        new Point3D(Max.X, Min.Y, Max.Z),
        new Point3D(Min.X, Max.Y, Max.Z),
        Max
        };
    }

    // Subdivisions de la forme
    public void Refine(List<IGeometricShape> aObj)
    {
        // Si le niveau de subdivision est de 0, ajouter le cube à la liste
        if (_subdivisionLevel <= 0)
        {
            aObj.Add(this);
            return;
        }

        // Taille des sous-cubes
        float newSizeX = (Max.X - Min.X) / 2;
        float newSizeY = (Max.Y - Min.Y) / 2;
        float newSizeZ = (Max.Z - Min.Z) / 2;

        // Sous-cubes
        for (int i = 0; i < 2; i++)
        { 
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Point3D newMin = new Point3D(
                        Min.X + i * newSizeX,
                        Min.Y + j * newSizeY,
                        Min.Z + k * newSizeZ
                    );

                    Point3D newMax = new Point3D(
                        Max.X + newSizeX,
                        Max.Y + newSizeY,
                        Max.Z + newSizeZ
                    );

                    var subCube = new Cube(newMin, newMax, _diffuseColor, _subdivisionLevel - 1);
                    subCube.Refine(aObj); // Appel récursif (raffine les sous-cubes)
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
