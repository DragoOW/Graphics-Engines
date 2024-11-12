
    public class Intersection
    {
        public bool HasIntersected { get; set; } // Indique une intersection trouvée
        public Point3D Point { get; set; } // Point d'intersection
        public Normal Normal { get; set; } // Normale à la surface à ce point
        public float T {  get; set; } // Temps d'intersection le long du rayon
        public IGeometricShape IntersectedObject { get; set; } // Objet intersecté

        public Intersection() 
        { 
            HasIntersected = false;
        }

        public Intersection(bool hasIntersected, Point3D point, Normal normal, float t, IGeometricShape intersectedObject)
        {
            HasIntersected = hasIntersected;
            Point = point;
            Normal = normal;
            T = t;
            IntersectedObject = intersectedObject;
        }
    }

