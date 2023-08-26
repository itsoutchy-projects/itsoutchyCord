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
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "settings.txt"), "client=stable");
            }
        }

        private void Webview_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
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
                    webview.ExecuteScriptAsync("document.addEventListener(\"load\", function(ev) {var styleSheet = document.createElement(\"style\"); styleSheet.innerText = " + File.ReadAllText(s) + "; document.head.appendChild(styleSheet)})");
                }

                // Inject plugins
                string[] plugins = Directory.GetFiles("plugins");
                foreach (string p in plugins)
                {
                    // This is a *lot* more straight-forward than themes
                    MessageBox.Show(File.ReadAllText(p));
                    webview.ExecuteScriptAsync(File.ReadAllText(p));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error injecting themes and/or plugins: " + ex.Message);
            }
        }
    }
}
