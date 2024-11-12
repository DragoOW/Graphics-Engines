
using System.Drawing; // Classe Point

public class GridAccelerator : IGeometricShape
{
    private class Cell
    {
        public List<IGeometricShape> m_objects = new List<IGeometricShape>();

        public void AddObject(IGeometricShape aObject)
        {
            m_objects.Add(aObject);
        }

        public BoundingBox ObjectBound()
        {
            // Récupérer la boîte englobante des objets dans la cellule
            BoundingBox cellBounds = m_objects[0].ObjectBound();

            foreach (var obj in m_objects)
            {
                cellBounds = cellBounds.Combine(obj.ObjectBound()); // Combiner avec celle des autres objets
            }

            return cellBounds;

        }

        public ulong Size() // Grands nombres positifs ou nulles
        {
            return (ulong)m_objects.Count;
        }

        public bool Intersect(Ray aRay, out float aT)
        {
            bool HasIntersected = false;
            float min_t = float.MaxValue;

            aT = float.MaxValue; // Par défaut, il n'y a pas d'intersection


            foreach (var obj in m_objects)
            {
                float tmpT; // Copie temporaire de t
                if (obj.Intersect(aRay, out tmpT) && tmpT < min_t)
                {
                    min_t = tmpT; // On met à jour
                    HasIntersected = true; // Intersection avec l'objet
                }

            }

            aT = min_t;
            return HasIntersected;
        }
    }

    private int[] m_nCells = new int[3];
    private BoundingBox m_bb;
    private Vector m_cellWidth;
    private Vector m_invCellWidth;
    private List<Cell> m_cells = new List<Cell>();

    public RGBColor DiffuseColor { get; set; }

    public GridAccelerator(List<IGeometricShape> aObj)
    {
        if (aObj.Count > 0)
        {
            // Volume englobant globale de tous les objets
            m_bb = aObj[0].ObjectBound();
            foreach (var obj in aObj)
            {
                m_bb = m_bb.Combine(obj.ObjectBound());
            }

            // Déterminer dynamiquement la taille des cellules selon le nombre d'objets dans la cellule
            int maxObjectsPerCell = 100;

            // Calculer les dimensions de la boîte englobante
            float widthX = m_bb.m_pMax.X - m_bb.m_pMin.X;
            float widthY = m_bb.m_pMax.Y - m_bb.m_pMin.Y;
            float widthZ = m_bb.m_pMax.Z - m_bb.m_pMin.Z;

            // Estimation du nombre de cellules dans chaque direction respectant le nombre d'objets dans une cellule
            m_nCells[0] = (int)Math.Ceiling(widthX / Math.Sqrt(aObj.Count));
            m_nCells[1] = (int)Math.Ceiling(widthY / Math.Sqrt(aObj.Count));
            m_nCells[2] = (int)Math.Ceiling(widthZ / Math.Sqrt(aObj.Count));

            // Calculer la taille de chaque cellule
            m_cellWidth = new Vector(
                widthX / m_nCells[0],
                widthY / m_nCells[1],
                widthZ / m_nCells[2]
            );

            m_invCellWidth = new Vector(1.0f / m_cellWidth.x, 1.0f / m_cellWidth.y, 1.0f / m_cellWidth.z);

            // Initialisation des cellules
            for (int i = 0; i < m_nCells[0]; i++)
            {
                for (int j = 0; j < m_nCells[1]; j++)
                {
                    for (int k = 0; k < m_nCells[2]; k++)
                    {
                        m_cells.Add(new Cell());
                    }
                }
            }


            // Répartition des objets dans les cellules
            foreach (var obj in aObj)
            {
                // Calculer la cellule correspondante en fonction de la position de l'objet
                BoundingBox objBound = obj.ObjectBound();
                int minX = SpaceToCell(objBound.m_pMin, 0);
                int minY = SpaceToCell(objBound.m_pMin, 1);
                int minZ = SpaceToCell(objBound.m_pMin, 2);

                // Calculer l'index de la cellule
                int cellIndex = minX + m_nCells[0] * (minY + m_nCells[1] * minZ);
                m_cells[cellIndex].AddObject(obj);

            }

        }
    }
    // Boîte englobante d'un objet
    public BoundingBox ObjectBound()
    {
        return m_bb;
    }

    // Boîte englobante du monde
    public BoundingBox worldBound()
    {
        // Calculer la boîte englobante du monde en comptant celles des cellules
        BoundingBox totalBounds = m_cells[0].ObjectBound();
        for (int i = 1; i < m_cells.Count; i++)
        {
            totalBounds = totalBounds.Combine(m_cells[i].ObjectBound());
        }

        return totalBounds;
    }

    public bool Intersect(Ray aRay, out float aT)
    {
        bool HasIntersected = false; // Par défaut, le rayon n'a pas d'intersection avec un objet ou avec une cellule
        aT = float.MaxValue;

        foreach (var cell in m_cells)
        {
            float tmpT; // Copie temporaire de T
            if (cell.Intersect(aRay, out tmpT))
            {
                if (tmpT < aT) // Si T temporaire est plus petit que aT  
                {
                    aT = tmpT; // On met à jour
                    HasIntersected = true; // La cellule a une intersection avec le rayon
                }
            }
        }

        return HasIntersected;
    }

    // (Repère monde -> repère cellule)
    public int SpaceToCell(Point3D aP, int aAxis)
    {
        // Accès explicite aux composantes de m_invCellWidth
        float cellWidth = 0;

        if (aAxis == 0) // Axe X
            cellWidth = m_invCellWidth.x;
        else if (aAxis == 1) // Axe Y
            cellWidth = m_invCellWidth.y;
        else if (aAxis == 2) // Axe Z
            cellWidth = m_invCellWidth.z;


        int v = (int)((aP.X - m_bb.m_pMin.X) * cellWidth);

        return Math.Max(0, Math.Min(v, m_nCells[aAxis] - 1));
    }

    // (Repère cellule -> repère monde)
    public double CellToSpace(int aP, int aAxis)
    {
        return m_bb.m_pMin[aAxis] + aP * m_cellWidth.x;
    }

    // Appliquer la transformation à chaque objet 
    public void ApplyTransformation(GeometricTransform transform)
    {
        foreach (var cell in m_cells)
        {
            foreach (var obj in cell.m_objects)
            {
                obj.ApplyTransformation(transform);
            }
        }
    }
    public Normal CalculateNormal(Point3D p)
    {
        return null;
    }

    public void Refine(List<IGeometricShape> aObj)
    {
    }
}

