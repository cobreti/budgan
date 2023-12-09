namespace Budgan;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var app = new Application(args);
            if (app.Init())
            {
                app.Run();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
