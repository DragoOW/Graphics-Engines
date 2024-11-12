using System.Numerics;

public class Vector
{
    public float x;
    public float y;
    public float z;

    public Vector(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public void Normalize()
    {
        float SquareNormalized = MathF.Sqrt(x * x + y * y + z * z);
        if (SquareNormalized == 0.0f)
        {
            throw new InvalidOperationException("Cannot normalize a zero-length vector");
        }

        x /= SquareNormalized; 
        y /= SquareNormalized;
        z /= SquareNormalized;
    }

    public float this[int index]
    {
        get 
        { 
            switch (index)
            {
                case 0: return x;
                case 1: return y;
                case 2: return z;
                default: throw new ArgumentOutOfRangeException(nameof(index), "Index should be 0, 1 or 2");
            }
        }
        set
        {
            switch(index) 
            { 
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index), "Index should be 0, 1 or 2");
            }

        }
     }


    public static Vector Cross(Vector v1, Vector v2)
    {
        return new Vector(v1.y * v2.z - v1.y * v2.z, v1.y * v2.z - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
    }

    public static float Dot(Vector v1 , Vector v2)
    {
        return v1.x * v2.x + v1.y * v2.z + v1.z * v2.z;
    }

    public Vector Add(Vector v)
    {
        return new Vector(x + v.x, y + v.y, z + v.z);
    }

    public Vector Scale(float scalar)
    {
        return new Vector(x * scalar, y * scalar, z * scalar);
    }

   
}

public class GeometricTransform
{
        // Variables
        public Matrix4x4 m_mat;
        public Matrix4x4 m_inv;
        public float theta;

        // Constructeur par defaut
        public GeometricTransform()
        {
            m_mat = Matrix4x4.Identity; 
            m_inv = Matrix4x4.Identity; 
        }
        
        // Constructeur (avec une transformation geometrique)
        public GeometricTransform(GeometricTransform a_gt)
        {
            if (a_gt == null)
            {
            throw new ArgumentNullException(nameof(a_gt), "Copied object cannot be NULL.");
            }

            //Copier les matrices
            m_mat = a_gt.m_mat;
            m_inv = a_gt.m_inv;
        }

        // Constructeur (avec une matrice 4x4 quelqueconque)
        public GeometricTransform(Matrix4x4 a_mat)
        {
           m_mat = a_mat;

            // Vu qu'Invert est un booleen, on ne fait que verifier si la matrice est inversible
            if (!Matrix4x4.Invert(m_mat, out m_inv)) 
            {
                throw new InvalidOperationException("The matrix cannot be inverted.");
            }
        }
        
        // Constructeur (avec une matrice 4x4 et sa matrice inverse)
        public GeometricTransform(Matrix4x4 mat, Matrix4x4 inv)
        {
            m_mat = mat;
            m_inv = inv;
        }
        
        // Appliquer une transformation sur un vecteur
        public Vector ApplyTransformationOnVector(Vector v)
        {
            // Convertir les coordonnées cartésiennes en coordonnées homogènes (h = 0)
            float[] homogeneous = new float[] { v.x, v.y, v.z, 0.0f };

            // Appliquer la transformation
            float x = m_mat.M11 * homogeneous[0] + m_mat.M12 * homogeneous[1] + m_mat.M13 * homogeneous[2] + m_mat.M14 * homogeneous[3];
            float y = m_mat.M21 * homogeneous[0] + m_mat.M22 * homogeneous[1] + m_mat.M23 * homogeneous[2] + m_mat.M24 * homogeneous[3];
            float z = m_mat.M31 * homogeneous[0] + m_mat.M32 * homogeneous[1] + m_mat.M33 * homogeneous[2] + m_mat.M34 * homogeneous[3];
            float h = m_mat.M41 * homogeneous[0] + m_mat.M42 * homogeneous[1] + m_mat.M43 * homogeneous[2] + m_mat.M44 * homogeneous[3];

            return new Vector(x, y, z);

        }

        // Appliquer une transformation sur un point
        public Point3D ApplyTransformationOnPoint(Point3D p)
        {
            Vector3 transformed = Vector3.Transform(p.ToVector3(), m_mat);
            return new Point3D(transformed.X, transformed.Y, transformed.Z);
        }

       public Normal ApplyTransformationOnNormal(Normal n)
       {
          // Matrice S 
          Matrix4x4 S = Matrix4x4.Transpose(m_inv); // Puisque S = (M^-1)^t
          
          // Appliquer S sur la normale
          float x = n.X * S.M11 + n.Y * S.M21 + n.Z * S.M31;
          float y = n.X * S.M12 + n.Y * S.M22 + n.Z * S.M32;
          float z = n.X * S.M13 + n.Y * S.M23 + n.Z * S.M33;

          // Retourner la normale transformée
          return new Normal(x, y, z);   
       }

    public BoundingBox ApplyTransformationOnBoundingBox(BoundingBox box)
    {
        // Créer une nouvelle boîte pour stocker les coins transformées
        BoundingBox transformedBox = new BoundingBox();

        // 8 coins d'une boîte englobante
        Point3D[] corners = new Point3D[8];
        corners[0] = box.m_pMin;
        corners[1] = new Point3D(box.m_pMax.X, box.m_pMin.Y, box.m_pMin.Z);
        corners[2] = new Point3D(box.m_pMin.X, box.m_pMax.Y, box.m_pMin.Z);
        corners[3] = new Point3D(box.m_pMax.X, box.m_pMax.Y, box.m_pMin.Z);
        corners[4] = new Point3D(box.m_pMin.X, box.m_pMin.Y, box.m_pMax.Z);
        corners[5] = new Point3D(box.m_pMax.X, box.m_pMin.Y, box.m_pMax.Z);
        corners[6] = new Point3D(box.m_pMin.X, box.m_pMax.Y, box.m_pMax.Z);
        corners[7] = new Point3D(box.m_pMax.X, box.m_pMax.Y, box.m_pMax.Z);

        // Transformer chaque coin et combiner les résultats
        foreach (Point3D corner in corners)
        {
            Point3D transformedCorner = ApplyTransformationOnPoint(corner);
            transformedBox = transformedBox.Combine(transformedCorner);

        }

        return transformedBox;
    }
    
    // Verifier que la matrice de transformation est une matrice identité
    public bool IsIdentity()
    {
       if(m_mat == Matrix4x4.Identity)
       {
            return true;
       }
       else
       {
            return false;
       }
    }

    public Matrix4x4 getMatrix()
    {
        return m_mat;
    }

    public Matrix4x4 getInverseMatrix()
    {
        return m_inv;
    }

    // Translation (Transformation géométrique #1)
    public GeometricTransform Translate(Vector v)
    {
        // Créer la matrice de transformation 
        m_mat *= Matrix4x4.CreateTranslation(v.x, v.y, v.z); 
        
        // Créer la matrice inverse 
        m_inv *= Matrix4x4.CreateTranslation(-v.x, -v.y, -v.z);     

        return new GeometricTransform(m_mat, m_inv);

    }


    // Propriété 1: Démontrer T(x1, y1, z1)T(x2, y2, z2) = T(x1 + x2, y1 + y2, z1 + z2)
    public static GeometricTransform CombineTranslations(Vector v1, Vector v2)
    {
        Vector combinedVector = new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        Matrix4x4 T1 = Matrix4x4.CreateTranslation(v1.x, v1.y, v1.z);
        Matrix4x4 T2 = Matrix4x4.CreateTranslation(v2.x, v2.y, v2.z);
        Matrix4x4 combinedMatrix = T1 * T2;
        return new GeometricTransform(combinedMatrix);

    }

    // Propriété 2: Démontrer T(x1, y1, z1) T(x2, y2, z2) = T(x2, y2, z2) T(x1, y1, z1)
    public static bool AreTranslationsCommutative(Vector v1, Vector v2)
    {
        Matrix4x4 T1 = Matrix4x4.CreateTranslation(v1.x, v1.y, v1.z);
        Matrix4x4 T2 = Matrix4x4.CreateTranslation(v2.x, v2.y, v2.z);
        Matrix4x4 product1 = T1 * T2; // T1 * T2
        Matrix4x4 product2 = T2 * T1; // T2 * T1
        return product1 == product2;    // Retourne toujours vrai

    }

    // Propriété 3: Démontrer que T(0, 0, 0) = I
    public bool NullVectorTranslation(Vector v)
    {
        if (v.x == 0 && v.y == 0 && v.z == 0)
        { 
            GeometricTransform nullTransform = Translate(v);
            return nullTransform.getMatrix() == Matrix4x4.Identity;
        }
        else
        {
            return false;
        }
    }
    

    // Changement d'échelle (Transformation geométrique)
    public GeometricTransform Scale(float x, float y, float z)
    {
        // Créer la matrice de transformation
        m_mat *= Matrix4x4.CreateScale(x, y, z);         


        // Créer la matrice inverse de transformation
        m_inv *= Matrix4x4.CreateScale(1 / x, 1 / y, 1 / z); 

        PrintMatrix(m_mat);

        return new GeometricTransform(m_mat, m_inv);
    }

    // Propriété 1: Démontrer que S(1, 1, 1) = I
    public bool OneVectorScale(Vector v)
    {
        if(v.x == 1 && v.y == 1 && v.z == 1)
        {
            GeometricTransform oneTransform = Scale(v.x, v.y, v.z);
            return oneTransform.getMatrix() == Matrix4x4.Identity;
        }
        else
        {
            return false;
        }
    }

    // Propriété 2 : S(x1, y1, z1) S(x2, y2, z2) = S(x1x2, y1y2, z1z2)
    public static GeometricTransform AreScalesAssociative(Vector v1, Vector v2)
    {
         Matrix4x4 S1 = Matrix4x4.CreateScale(v1.x, v1.y, v1.z);
         Matrix4x4 S2 = Matrix4x4.CreateScale(v2.x, v2.y, v2.z);
         Matrix4x4 product = S1 * S2;
        return new GeometricTransform(product);
    }

    // Rotation (X)
    public GeometricTransform RotateX(float theta)
    {
        // Créer la matrice de transformation
                                                        
        m_mat *= Matrix4x4.CreateRotationX(theta);

        // Créer la matrice inverse de transformation
        m_inv *= Matrix4x4.Transpose(m_mat);

        PrintMatrix(m_mat);

        return new GeometricTransform(m_mat, m_inv);
    }

    // Rotation (Y)
    public GeometricTransform RotateY(float theta)
    {
        // Créer la matrice de transformation
                                                        
        m_mat *= Matrix4x4.CreateRotationY(theta);

        // Créer la matrice inverse de transformation
        m_inv *= Matrix4x4.Transpose(m_mat);

        PrintMatrix(m_mat);

        return new GeometricTransform(m_mat, m_inv);
    }

    // Rotation (Z)
    public GeometricTransform RotateZ(float theta)
    {
        // Créer une matrice de transformation
                                                     
        m_mat *= Matrix4x4.CreateRotationZ(theta);

        // Créer une matrice inverse de transformation
        m_inv *= Matrix4x4.Transpose(m_mat);

        PrintMatrix(m_mat);

        return new GeometricTransform(m_mat, m_inv);
    }

    static void PrintMatrix(Matrix4x4 matrix)
    {
        for (int i = 0; i < 4; i++)
        {
            for(int j = 0;  j < 4; j++)
            {
                Console.Write($"{matrix[i, j]:F3} ");
            }
            Console.WriteLine();
        }
    }

    // Rotation autour d'un axe arbitraire (appliqué à un vecteur v)
    public Vector rotate(Vector v, float theta, Vector d)
    {
        // Normaliser le vecteur direction
        d.Normalize();

        // Évaluer la projection v_d
        float ProjectionLength = Vector.Dot(v, d);
        Vector v_d = d.Scale(ProjectionLength);

        // v1 = v - v_d (Vecteur perpendiculaire)
        Vector v1 = v.Add(v_d.Scale(-1));

        // v2 = v1 x d (Produit vectoriel)
        Vector v2 = Vector.Cross(v1, d);

        // Calculer la rotation (angle theta)
        float cosTheta = MathF.Cos(theta);
        float sinTheta = MathF.Sin(theta);

        // Vecteur résultant
        Vector vPrime = v_d.Add(v1.Scale(cosTheta)).Add(v2.Scale(sinTheta));

        return vPrime;
    }

    // Matrice de transformation de la rotation autour d'un axe arbitraire
    public Matrix4x4 GetRotationMatrix(float theta,  Vector d)
    {
        // Normalisation du vecteur direction
        d.Normalize();

        // Composants pour créer la matrice de rotation
        float cosTheta = MathF.Cos(theta);
        float sinTheta = MathF.Sin(theta);
        float x = d.x;
        float y = d.y;
        float z = d.z;

        // Vecteurs de base
        Vector3 e_x = new Vector3(1, 0, 0);
        Vector3 e_y = new Vector3(0, 1, 0);
        Vector3 e_z = new Vector3(0, 0, 1);

        // Appliquer la rotation sur les vecteurs
        Vector3 rotated_x = Vector3.Transform(e_x, Matrix4x4.CreateRotationX(theta));
        Vector3 rotated_y = Vector3.Transform(e_y, Matrix4x4.CreateRotationY(theta));
        Vector3 rotated_z = Vector3.Transform(e_z, Matrix4x4.CreateRotationZ(theta));

        // Créer la matrice de rotation
        var rotationMatrix = new Matrix4x4(rotated_x.X, rotated_y.X, rotated_z.X, 0, rotated_x.Y, rotated_y.Y, rotated_z.Y, 0, rotated_x.Z, rotated_y.Z, rotated_z.Z, 0, 0, 0, 0, 1);

        return rotationMatrix;
    }

    // Matrice inverse de la matrice de transformation de rotation autour d'un axe arbitraire
    public Matrix4x4 GetInverseRotationMatrix(Matrix4x4 rotationMatrix)
    {
        return Matrix4x4.Transpose(rotationMatrix);
    }

    
}

