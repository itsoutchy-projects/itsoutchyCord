using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace itsoutchyCord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // We need to only run injection code when the page loads
            webview.NavigationCompleted += Webview_NavigationCompleted;
            // Settings (clients only for now)
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "settings.txt")))
            {
                string[] prefs = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "settings.txt")).Split("\n");
                foreach (string pref in prefs)
                {
                    string[] assignment = pref.Split("=");
                    if (assignment[0] == "client")
                    {
                        if (assignment[1] != "stable")
                        {
                            webview.Source = new Uri("https://" + assignment[1] + ".discord.com/app");
                        }
                    }
                }
            } else
            {
                // Create the settings file with the defaults for next time, in case there somehow isn't one
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "settings.txt"), "client=stable");
            }
        }

        private async void Webview_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            logToConsole("WARNING: Do not use random code snippets! Make sure you can read the code, otherwise you're risking your account being stolen!"); // Dunno why people paste random code snippets into the console, well. I mean, some are good, but still.
            logToConsole("Started loading");
            // Give discord time to load, this is 4 seconds
            await Task.Delay(4000);
            // Injection code
            try
            {
                // Inject themes
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "themes"));
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "plugins"));

                string[] themes = Directory.GetFiles("themes");
                foreach (string s in themes)
                {
                    // This should be built-in, can't believe we need custom Javascript just to inject css
                    //await webview.ExecuteScriptAsync("document.addEventListener(\"load\", function(ev) {var styleSheet = document.createElement(\"style\"); styleSheet.innerText = " + File.ReadAllText(s) + "; document.head.appendChild(styleSheet)})");
                    await webview.injectCSS(File.ReadAllText(s));
                }

                // Inject plugins
                string[] plugins = Directory.GetFiles("plugins");
                foreach (string p in plugins)
                {
                    // This is a *lot* more straight-forward than themes
                    await webview.ExecuteScriptAsync(File.ReadAllText(p));
                }
                logToConsole("Finished injection");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error injecting themes and/or plugins: " + ex.Message);
            }
        }

        public void logToConsole(string message)
        {
            webview.ExecuteScriptAsync("console.log(\"" + message + "\");");
        }
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// Inject css into a <see cref="WebView2"/>
        /// </summary>
        /// <param name="theView"></param>
        /// <param name="css">The css code to inject</param>
        /// <returns></returns>
        public async static Task<string> injectCSS(this WebView2 theView, string css)
        {
            // Why not make this built-in?
            return await theView.ExecuteScriptAsync("document.addEventListener(\"load\", function(ev) {var styleSheet = document.createElement(\"style\"); styleSheet.innerText = " + css + "; document.head.appendChild(styleSheet)})");
        }
    }
}
