public class UniformSampler : Sampler
{
    public UniformSampler()
    {
        // Création d'un échantillonneur
        Sampler sampler = new Sampler();
    }

    public UniformSampler(int a_nbSamples)
    {
        // Création d'un échantillonneur
        Sampler sampler = new Sampler(a_nbSamples);
        // Initialisation des variables
        m_nbSamples = a_nbSamples;
        // Générer des échantillons uniformément
        GenerateSamples();
    }

    public UniformSampler(int a_nbSamples, int a_nbSets)
    {
        // Création d'un échantillonneur
        Sampler sampler = new Sampler(a_nbSamples, a_nbSets);
        // Initialisation des variables
        m_nbSamples = a_nbSamples;
        m_nbSets = a_nbSets;
        // Générer des échantillons uniformément
        GenerateSamples();
    }

    public UniformSampler(UniformSampler a_us)
    {
        // Création d'un échantillonneur
        Sampler sampler = new Sampler(a_us);
        // Initialisation des variables
        m_nbSamples = a_us.m_nbSamples;
        // Générer des échantillons uniformément
        GenerateSamples();
    }

    private void GenerateSamples()
    {
        int n = (int)Math.Sqrt(m_nbSamples);
        for (int s = 0; s < m_nbSets; s++)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    m_samples.Add(new Point3D((i + 0.5f) / n, (j + 0.5f) / n, 0));
                }
            }
        }
    }
}

