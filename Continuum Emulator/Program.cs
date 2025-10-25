using System;

namespace Continuum93
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Continuum(args).Run();
        }
    }
}
