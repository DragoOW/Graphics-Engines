
public class Accelerator : IGeometricShape
{
    protected List<IGeometricShape> m_objects;
    private Ray currentRay;
    private RGBColor _diffuseColor;
    
    public void Refine(List<IGeometricShape> aObj)
    {
        foreach (var obj in m_objects)
        {
            if (obj.Intersect(currentRay, out float t))
            {
                aObj.Add(obj);
            }
            else
            {
                obj.Refine(aObj);
            }
        }
    }


    public Accelerator()
    {
        m_objects = new List<IGeometricShape>();
        _diffuseColor = new RGBColor(1.0f, 1.0f, 1.0f);
    }

    // Couleur de l'objet intersecté
    public RGBColor DiffuseColor 
    {
        get => _diffuseColor; // Retourne la couleur par défaut
        
        set => _diffuseColor = value;      
    }

    public bool Intersect(Ray ray, out float t)
    {
        currentRay = ray;
        t = float.PositiveInfinity;
        IGeometricShape closestObject = null; // Garder l'objet le plus proche en mémoire
        bool HasIntersected = false;

        foreach (var obj in m_objects) // Pour chaque forme dans la liste de formes
        {
            if (obj.Intersect(ray, out float tempT)) // Si la forme intersecte avec un rayon
            {
                HasIntersected = true;
                if(tempT < t)  
                {
                    t = tempT; // Met à jour t avec la plus petite distance
                    closestObject = obj; // Met à jour avec l'objet le plus proche
                }
            }
        }

        if (closestObject != null)
        {
            _diffuseColor = closestObject.DiffuseColor; // Utiliser la couleur de l'objet le plus proche
        }

        return HasIntersected; // Retourne vrai s'il y a au moins une intersection, sinon retourne faux
    }

    /* Ces fonctions font partie de l'interface GeometricShape mais ne sont pas nécessaires pour la 
     classe Accelerator */
    public void ApplyTransformation(GeometricTransform transform)
    {
    }

    public Normal CalculateNormal(Point3D p)
    {
        return null;
    }

    public BoundingBox ObjectBound()
    {
        return null;
    }
}
