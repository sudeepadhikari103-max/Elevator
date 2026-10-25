// Entry Point of Windows Form Application

namespace elevatorsystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Elevatorsystem());
        }
    }
}
