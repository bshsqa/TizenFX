using System;

namespace NUITestSample
{
    class Application
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            new HelloWorldTest.Example().Run(args);
        }
    }
}
