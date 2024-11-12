using System.Numerics;

public class RayTracer
{
    private World world;

    public RayTracer(World world)
    {
        this.world = world;
    }

    public void RenderScene()
    {
        float s = world.m_vp.PixelSize; // Taille d'un pixel
        int xres = world.m_vp.XRes;     // Résolution horizontale
        int yres = world.m_vp.YRes;     // Résolution verticale

        // Accéder à l'objet Sampler
        Sampler sampler = new Sampler(world.m_vp.NumSamples); // Le nombre d'échantillons par pixel

        // Parcourir tous les pixels du plan de l'image
        for (int j = 0; j < yres; j++)  // Parcours des pixels en hauteur (axe Y)
        {
            for (int i = 0; i < xres; i++)  // Parcours des pixels en largeur (axe X)
            {
                // Initialisation de la couleur du pixel (noir)
                RGBColor pixelColor = new RGBColor(0f, 0f, 0f);

                // Moyenne des couleurs des rayons lancés sur le pixel
                for (int k = 0; k < sampler.GetNbSamples(); k++)
                {
                    // Échantillonner un point dans l'espace des coordonnées [0, 1]²
                    Point3D sample = sampler.SampleUnitSquare();

                    // Calculer la position du point (x, y) sur le plan image
                    float x = s * (i - (xres + 1) / 2.0f);
                    float y = -s * (j - (yres - 1) / 2.0f);

                    // Origine du rayon = point sur le plan image
                    Point3D origin = new Point3D(x, y, 0.0f);  // z = 0 pour le plan de l'image

                    // Direction du rayon = vers l'avant (0, 0, -1)
                    Normal direction = new Normal(0.0f, 0.0f, -1.0f);


                    // Créer le rayon r(t) = origin + t * direction
                    Ray ray = new Ray(origin, direction);

                    // Tracer le rayon et déterminer la couleur du pixel
                    RGBColor rayColor = Trace(ray);

                    // Ajouter la couleur retournée par le rayon à la couleur du pixel
                    pixelColor += rayColor;
                }

                // Moyenne des couleurs des rayons pour le pixel
                pixelColor /= sampler.GetNbSamples();

                // Définir la couleur du pixel dans l'image
                world.RenderImage.SetPixel(i, yres - j - 1, pixelColor.ToColor());
            }
        }

        // Sauvegarder l'image
        world.RenderImage.Save("output.png");
    }

    public RGBColor Trace(Ray ray)
    {
        float t;
        RGBColor color = world.m_backgroundColor; // Pas d'intersection, couleur de fond (noir)
        Console.WriteLine("Aucune intersection trouvée.");
        RGBColor intersectedColor = null;

        foreach (var shape in world.Shapes)
        {

            if (shape.Intersect(ray, out t))
            {
                Console.WriteLine($"Intersection trouvée avec {shape.GetType().Name} à t = {t}");
                Point3D Pinter = ray.Call(t); // r(t) = origine + t * direction
                Normal n = shape.CalculateNormal(Pinter);

                // Direction de la lumière
                Vector3 lightDirection = new Vector3(0, 0, 1);
                // Normalisation
                Vector3 Normalized_vec = n.ToVector3();
                // Produit scalaire (entre la direction de la lumière et la normale)
                float dotProduct = MathF.Max(0, Vector3.Dot(Normalized_vec, lightDirection));

                // Stocker la couleur désigné
                intersectedColor = shape.DiffuseColor;

                // Calculer la couleur selon le produit scalaire
                color = dotProduct * intersectedColor;
                
                // On arrête la boucle après la première intersection 
                break;
            }

        }

        return color;
        
    }

}
