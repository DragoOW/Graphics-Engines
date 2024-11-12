
public class Sampler
{
    protected int m_nbSamples;
    protected int m_nbSets;
    protected List<Point3D> m_samples;
    protected List<int> m_shuffledIndices;
    protected List<Point3D> m_circleSamples;
    protected List<Point3D> m_hemisphereSamples;
    protected ulong m_count;
    protected int m_jump;

    public Sampler()
    {
        // Initialisation des variables
        m_nbSamples = 1;
        m_nbSets = 83;
        m_count = 0;
        m_jump = 0;

        // Réservation de l'espace pour m_samples
        m_samples = new List<Point3D>(m_nbSamples * m_nbSets);

        SetupShuffledIndices();
    }

    public Sampler(int a_nbSamples)
    {
        // Initialisation des variables
        m_nbSamples = a_nbSamples;
        m_nbSets = 83;
        m_count = 0;
        m_jump = 0;

        // Réservation de l'espace pour m_samples
        m_samples = new List<Point3D>(m_nbSamples * m_nbSets);

        SetupShuffledIndices();
    }
    // Constructeur avec un nombre d'échantillons et d'ensembles d'échantillons donné
    public Sampler(int a_nbSamples, int a_nbSets)
    {
        // Initialisation des variables
        m_nbSamples = a_nbSamples;
        m_nbSets = a_nbSets;
        m_count = 0;
        m_jump = 0;

        // Réservation de l'espace pour m_samples
        m_samples = new List<Point3D>(m_nbSamples * m_nbSets);

        SetupShuffledIndices();
    }
    // Constructeur par copie
    public Sampler(Sampler a_s)
    {
        m_nbSamples = a_s.m_nbSamples;
        m_nbSets = a_s.m_nbSets;
        m_samples = new List<Point3D>(a_s.m_samples);
        m_shuffledIndices = new List<int>(a_s.m_shuffledIndices);
        m_circleSamples = new List<Point3D>(a_s.m_circleSamples);
        m_hemisphereSamples = new List<Point3D>(a_s.m_hemisphereSamples);
        m_count = a_s.m_count;
        m_jump = a_s.m_jump;
    }
    // Constructeur d'affectation
    public void CopyFrom(Sampler a_s)
    {
        if (this == a_s)
            return;

        m_nbSamples = a_s.m_nbSamples;
        m_nbSets = a_s.m_nbSets;

        m_samples = new List<Point3D>(a_s.m_samples);
        m_shuffledIndices = new List<int>(a_s.m_shuffledIndices);
        m_circleSamples = new List<Point3D>(a_s.m_circleSamples);
        m_hemisphereSamples = new List<Point3D>(a_s.m_hemisphereSamples);

        m_count = a_s.m_count;
        m_jump = a_s.m_jump;
    }

    public void SetNbSets(int a_nbSets)
    {
        m_nbSets = a_nbSets;
    }
    // Échantillonnage sur un carré unitaire
    public Point3D SampleUnitSquare()
    {
        if ((int)m_count % m_nbSamples == 0) // Si tous les échantillons sont parcourus
        {
            // Réinitialisation du générateur de nombre aléatoires
            Random rdmTimeBased = new Random((int)DateTime.Now.Ticks);
            m_jump = rdmTimeBased.Next(m_nbSets) * m_nbSamples;
        }

        // Sélectionne l'échantillon selon le jump
        var index = m_jump + m_shuffledIndices[m_jump + (int)m_count % m_nbSamples];
        m_count++;

        return m_samples[index]; // Retourne l'échantillon
    }
    // Échantillonnage sur un cercle unitaire
    public Point3D SampleUnitCircle()
    {
        if ((int)m_count % m_nbSamples == 0) // Si tous les échantillons sont parcourus
        {
            // Réinitialisation du générateur de nombre aléatoires
            Random rdmTimeBased = new Random((int)DateTime.Now.Ticks);
            m_jump = rdmTimeBased.Next(m_nbSets) * m_nbSamples;
        }

        // Sélectionne l'échantillon selon le jump
        var index = m_jump + m_shuffledIndices[m_jump + (int)m_count % m_nbSamples];
        m_count++;

        return m_samples[index]; // Retourne l'échantillon
    }
    // Échantillonnage sur une demi-sphère unitaire
    public Point3D SampleUnitHemisphere()
    {
        if ((int)m_count % m_nbSamples == 0) // Si tous les échantillons sont parcourus
        {
            // Réinitialisation du générateur de nombre aléatoires
            Random rdmTimeBased = new Random((int)DateTime.Now.Ticks);
            m_jump = rdmTimeBased.Next(m_nbSets) * m_nbSamples;
        }

        // Sélectionne l'échantillon selon le jump
        var index = m_jump + m_shuffledIndices[m_jump + (int)m_count % m_nbSamples];
        m_count++;

        return m_samples[index]; // Retourne l'échantillon
    }
    // Mapping carré -> cercle
    public void MapSquare2Circle()
    {
        // On applique le technique des régions concentriques

        
        foreach (var sample in m_samples)
        {
            // Convertir les échantillons du domaine [0, 1]² vers [-1, 1]²
            float xTransformed = 2.0f * sample.X - 1.0f; // x = 0 -> x = -1
            float yTransformed = 2.0f * sample.Y - 1.0f; // y = 1 -> y = 1

            // Initialiser le rayon et l'angle
            float r = 0.0f;
            float theta = 0.0f;

            // Déterminer le rayon et l'angle selon le quartier
            if (xTransformed > yTransformed && xTransformed > -yTransformed) 
            {
                r = xTransformed;
                theta = (MathF.PI / 4) * (yTransformed / xTransformed);
            }
            else if (xTransformed < yTransformed && xTransformed > -yTransformed)
            {
                r = yTransformed;
                theta = (MathF.PI / 4) * (2 - (xTransformed / yTransformed));
            }
            else if (xTransformed < yTransformed && xTransformed < -yTransformed)
            {
                r = -xTransformed;
                theta = (MathF.PI / 4) * (4 + (yTransformed / xTransformed));
            }
            else if (xTransformed > yTransformed && xTransformed < -yTransformed)
            {
                r = -yTransformed;
                theta = (MathF.PI / 4) * (6 - (xTransformed / yTransformed));
            }

            // Projection sur le cercle unitaire
            if (r > 1.0f)
            {
                r = 1.0f;
            }

            // Calcul des coordonnées sur la surface du cercle unitaire
            sample.X = r * MathF.Cos(theta);
            sample.Y = r * MathF.Sin(theta);
            sample.Z = 0.0f;

            // Stocker les coordonnées polaires
            m_circleSamples.Add(new Point3D(r, theta, 0.0f));
        }
    }
    // Mapping carré -> demisphère
    public void MapSquare2Hemisphere(float a_alpha)
    {
        foreach (var sample in m_samples)
        {
            // Convertir les échantillons du domaine [0, 1]² vers [-1, 1]²
            float xTransformed = 2.0f * sample.X - 1.0f; // x = 0 -> x = -1
            float yTransformed = 2.0f * sample.Y - 1.0f; // y = 1 -> y = 1 

            // Calcul de φ et θ
            float phi = 2.0f * MathF.PI * sample.X; //φ = 2πx
            float theta = MathF.Acos(MathF.Pow(1.0f - sample.Y, (1.0f / (a_alpha + 1.0f)))); // θ = cos^(-1)((1 - y)^(1/(α + 1)))

            // Projection sur la surface de la demi-sphère unitaire
            float z = MathF.Cos(theta); // z = cos(θ)
            float r = MathF.Sin(theta); // r = sin(θ)

            // Calcul des coordonnées cartésiennes sur la demi-sphère
            float x = r * MathF.Cos(phi); // x = r * cos(φ)
            float y = r * MathF.Sin(phi); // y = r * sin(φ)

            // Construction du repère local uvw
            var w = new Vector(x, y, z); // Direction radiale
            w.Normalize();

            Vector v0 = new Vector(1f, 0f, 0f); // Vecteur arbitraire pour u
            if (MathF.Abs(w.x) > 0.99999f) // Si w est proche du vecteur arbitraire, utiliser un autre vecteur arbitraire
            {
                v0 = new Vector(0f, 1f, 0f);
            }
            var u = Vector.Cross(v0, w); // u = v0 x w
            u.Normalize();

            var v = Vector.Cross(w, u); // v = w x u
            v.Normalize();

            // Ajout des coordonnées cartésiennes sur la demi-sphère unitaire
            m_hemisphereSamples.Add(new Point3D(x, y, z));
        }
    }
    private void SetupShuffledIndices()
    {
        m_shuffledIndices = new List<int>(m_nbSamples * m_nbSets); // m_nbSamples * m_nbSet calcule les indices
        for (int i = 0; i < m_shuffledIndices.Capacity; i++)
        {
            m_shuffledIndices.Add(i); // Ajoute des indices à la liste
        }

        Shuffle(m_shuffledIndices);
    }

    private void Shuffle(List<int> list)
    {
        Random rdm = new Random();
        int n = list.Count; // n est le nombre d'élément dans la liste
        while (n > 1)
        {
            n--;
            int k = rdm.Next(n + 1); // k est l'indice aléatoire de n + 1
            int value = list[k];
            list[k] = list[n]; // On mélange les indices (k -> n)
            list[n] = value;
        }
    }

    public int GetNbSamples()
    {
        return m_nbSamples;
    }

}

