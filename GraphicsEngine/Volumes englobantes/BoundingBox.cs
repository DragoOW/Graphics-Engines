 public class BoundingBox
 {
    // Attributs
    public Point3D m_pMin;
    public Point3D m_pMax;

    // Constructeur (boîte vide)

    public BoundingBox()
    {
        // Initialiser les coins de la boîte avec des valeurs infinies
        m_pMin = new Point3D(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        m_pMax = new Point3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
    }

    // Constructeur (boîte avec un point)
    public BoundingBox(Point3D p)
    {
        m_pMin = p;
        m_pMax = p;
    }

    // Constructeur (boîte avec deux points)
    public BoundingBox(Point3D min, Point3D max)
    {
        m_pMin = min;
        m_pMax = max;
    }

    // Vérifier si deux boîtes se chevauchent
    public bool Overlaps(BoundingBox a_bb)
    {
        if(m_pMax.X < a_bb.m_pMin.X || m_pMin.X > a_bb.m_pMax.X)
        {
            return false; // Pas de chevauchement sur l'axe X
        }
        if(m_pMax.Y < a_bb.m_pMin.Y || m_pMin.Y > a_bb.m_pMax.Y)
        {
            return false; // Pas de chevauchement sur l'axe Y
        }
        if(m_pMax.Z < a_bb.m_pMin.Z || m_pMin.Z > a_bb.m_pMax.Z)
        {
            return false; // Pas de chevauchement sur l'axe Z
        }
        else
        {
            return true; // Il a chevauchement sur tous les axes
        }
    }

    public bool Contains(Point3D a_p)
    {
        return (a_p.X >= m_pMin.X && a_p.X <= m_pMax.X) &&
               (a_p.Y >= m_pMin.Y && a_p.Y <= m_pMax.Y) &&
               (a_p.Z >= m_pMin.Z && a_p.Z <= m_pMax.Z);
        // Retourne true si toutes les conditions sont respectées sinon retourne false
    }

    public bool Intersect(Ray a_r, double a_t0, double a_t1)
    {
        double t0 = a_t0; // Stocke la valeur de t0
        double t1 = a_t1; // Stocke la valeur de t1

        // Pour chaque tranche d'axe, on regarde si le rayon touche une des tranches
        for (int axis = 0; axis < 3; axis++)
        {
            double min = (axis == 0) ? m_pMin.X : (axis == 1) ? m_pMin.Y : m_pMin.Z; /* On cherche la valeur min de la boîte, si le bas de la boîte touche l'axe,
                                                                                      * on rend la valeur de l'axe sinon on rend la coordonnée minimale de la boîte selon l'axe
                                                                                      * le même principe s'applique sur les autres axes, jusqu'à ce qu'on trouve le min.
                                                                                      */

            double max = (axis == 0) ? m_pMax.X : (axis == 1) ? m_pMax.Y : m_pMax.Z; /* On cherche la valeur max de la bôite, si le haut de la boîte touche l'axe,
                                                                                      * on rend la valeur de l'axe sinon on rend la coordonnée maximale de la boîte selon l'axe
                                                                                      * le même principe s'applique sur les autres axes, jusqu'à ce qu'on trouve le max
                                                                                      */

            double origin = (axis == 0) ? a_r.Origin.X : (axis == 1) ? a_r.Origin.Y : a_r.Origin.Z; /* On cherche la valeur de l'origine du rayon, si l'origine du rayon touche l'axe,
                                                                                                     * on rend la valeur de l'axe sinon on rend les coordonnées de l'origine du rayon selon l'axe
                                                                                                     * le même principe s'applique sur les autres axes, jusqu'à ce qu'on trouve le max
                                                                                                     */
            double direction = (axis == 0) ? a_r.Direction.X : (axis == 1) ? a_r.Direction.Y : a_r.Direction.Z; /* On cherche la valeur de la direction du rayon, si le rayon a la même direction que l'axe,
                                                                                                                 * on rend la valeur de l'axe sinon on rend la direction du rayon selon l'axe
                                                                                                                 * le même principe s'applique sur les autres axes, jusqu'à ce qu'on trouve le max
                                                                                                                 */

            if (direction == 0)
            {
                // Le rayon est parallèle à l'axe
                if (origin < min || origin > max)
                {
                    return false; // Pas d'intersection
                }
            }
            else
            {
                double invDir = 1.0 / direction; // Inverse de la direction pour éviter les divisions par zéro
                double tNear = (min - origin) * invDir; // Correspond à la plus petite valeur de l'intervalle 
                double tFar = (max - origin) * invDir; // Correspomd à la plus grande valeur de l'intervalle

                // S'assurer que tNear est toujours le plus petit
                if (tNear > tFar)
                {
                    double temp = tNear;
                    tNear = tFar;
                    tFar = temp;
                }

                // Mettre à jour l'intervalle global [t0, t1]
                t0 = Math.Max(t0, tNear);
                t1 = Math.Min(t1, tFar);

                // Verifier la validité de l'intervalle
                if (t0 > t1)
                {
                    return false; // Pas d'intersection
                }
            }
        }

        return true; // Retourne vrai si toutes les conditions sont réunis
    }

    public int maxAxis()
    {
        // Calculer les longueurs des axes
        float lengthX = m_pMax.X - m_pMin.X;
        float lengthY = m_pMax.Y - m_pMin.Y;
        float lengthZ = m_pMax.Z - m_pMin.Z;

        // Comparaison des longueurs 
        if(lengthX >= lengthY && lengthX >= lengthZ)
        {
            return 0; // Axe X;
        }
        if(lengthY >= lengthX && lengthY >= lengthZ)
        {
            return 1; // Axe Y;
        }
        else
        {
            return 2; // Axe Z;
        }
    }

    public BoundingBox Combine(BoundingBox a_bb)
    {
        Point3D newMin = new Point3D(Math.Min(m_pMin.X, a_bb.m_pMin.X), Math.Min(m_pMin.Y, a_bb.m_pMin.Y), Math.Min(m_pMin.Z, a_bb.m_pMin.Z)); // Créer un nouveau point min
        Point3D newMax = new Point3D(Math.Max(m_pMax.X, a_bb.m_pMax.X), Math.Max(m_pMax.Y, a_bb.m_pMax.Y), Math.Max(m_pMax.Z, a_bb.m_pMax.Z)); // Créer un nouveau point max

        return new BoundingBox(newMin, newMax); // Retourner une nouvelle boîte combinant les deux boîtes
    }

    public BoundingBox Combine(Point3D p)
    {
        Point3D newMin = new Point3D(Math.Min(m_pMin.X, p.X), Math.Min(m_pMin.Y, p.Y), Math.Min(m_pMin.Z, p.Z)); // Crée un nouveau point min
        Point3D newMax = new Point3D(Math.Max(m_pMax.X, p.X), Math.Max(m_pMax.Y, p.Y), Math.Max(m_pMax.Z, p.Z)); // Crée un nouveau point max

        return new BoundingBox(newMin, newMax); // Retourner une nouvelle boîte combinant une boîte et un point
    }
 }

