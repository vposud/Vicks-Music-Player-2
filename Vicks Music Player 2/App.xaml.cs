using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Vicks_Music_Player_2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "My_Unique_Application_String";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            var main = App.Current.MainWindow as MainWindow;
            main.SingleInstance(args[1]);

            return true;
        }
        #endregion
    }
}
