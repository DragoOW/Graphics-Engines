using System.Collections.Generic;
using System.Drawing;
using System.Numerics;


public class World
{
    public ViewPlane m_vp;
    public RGBColor m_backgroundColor;
    public List<IGeometricShape> Shapes {  get; private set; }
    public List<LightSource> LightSources { get; private set; }
    public Point3D CameraPosition { get; private set; }
    public Bitmap RenderImage { get; private set; }

    // Classe World - Configuration de la couleur d'arrière-plan
    public World()
    {
        // Caméra et plan de vue
        CameraPosition = new Point3D(0.0f, 0.0f, 0.0f);
        m_vp = new ViewPlane(800, 600, 1.0f, 1.0f, 250);

        // Couleur de fond (noir)
        m_backgroundColor = new RGBColor(0.0f, 0.0f, 0.0f);

        // Initialiser la liste de formes
        Shapes = new List<IGeometricShape>();

        // Cône
        Cone cone1 = new Cone(new Point3D(0.0f, 0.0f, -600.0f), 300.0f, 200.0f);
        GeometricTransform translationY1 = new GeometricTransform().Translate(new Vector(0.0f, 100.0f, 0.0f));
        cone1.ApplyTransformation(translationY1);
        GeometricTransform rotationX = new GeometricTransform().RotateX(MathF.PI / 2);
        cone1.ApplyTransformation(rotationX);
        Shapes.Add(cone1);

        // Lumière
        LightSources = new List<LightSource>
    {
        new LightSource(new Point3D(100.0f, 150.0f, -100.0f), new RGBColor(1.0f, 1.0f, 1.0f), 0.8f)
    };

        RenderImage = new Bitmap(m_vp.XRes, m_vp.YRes);
    }


    public void Build() { }

    public void RenderScene(string filename)
    {
        RayTracer tracer = new RayTracer(this);
        tracer.RenderScene();
        RenderImage.Save(filename);
    }
}
