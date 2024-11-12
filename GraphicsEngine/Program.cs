public class Program
{
    public static void Main()
    {
        // Création du monde
        World world = new World();
        world.Build();

        // Rendre la scène et sauvegarder l'image
        world.RenderScene("output.png");

        Console.WriteLine("Scène rendue et sauvegardée dans output.png");
    }
}
