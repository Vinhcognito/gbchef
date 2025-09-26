using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace gbchef
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            StackTrace trace = new(e.Exception, true);
            MessageBox.Show($"An unhandled exception just occurred: \n {e.Exception.GetType()} - {e.Exception.Message}", "Application Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;

            Debug.WriteLine($"=== UnhandledException ===");
            Debug.WriteLine(trace?.GetFrame(0)?.GetMethod()?.ReflectedType?.FullName);
            Debug.WriteLine("Line: " + trace?.GetFrame(0)?.GetFileLineNumber());
            Debug.WriteLine("Column: " + trace?.GetFrame(0)?.GetFileColumnNumber());
            Debug.WriteLine($"==========================");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }
    }

}
