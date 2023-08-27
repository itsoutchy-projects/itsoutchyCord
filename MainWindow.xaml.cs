using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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
        public static string localVersion = "v0.1-alpha";
        public static string? onlineVer;
        public MainWindow()
        {
            InitializeComponent();
            clearPLESESERKDEJKFJEJF();
            webview.Source = new Uri("https://discord.com/app");
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

        // name is because i was scared that my account would be gone due to caching
        async Task clearPLESESERKDEJKFJEJF()
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "itsoutchyCord"));
                var webView2Environment = await CoreWebView2Environment.CreateAsync(null, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "itsoutchyCord"));
                await webview.EnsureCoreWebView2Async(webView2Environment);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private async Task updateChecker(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // Check for updates
            HttpClient http = new HttpClient();
            http.BaseAddress = new Uri("https://raw.githubusercontent.com/itsoutchy-projects/itsoutchyCord/main/gitVersion.txt");
            HttpResponseMessage response = await http.GetAsync(http.BaseAddress);
            HttpContent content = response.Content;
            onlineVer = await content.ReadAsStringAsync();
            logToConsole(onlineVer);
            if (localVersion != onlineVer)
            {
                // Show the notification for 7 seconds before hiding again
                logToConsole("Should be shown");
                // Stupid Webview2 hiding all other controls for absolutely no reason!!!!!!!!!!
                // Fixed sorta by using a seperate window with no titlebar and it doesn't show in the taskbar, I don't think it moves with the main window though which is annoying
                updateNotif notif = new updateNotif();
                notif.Top = Top;
                notif.Left = Left;
                notif.Show();
                logToConsole("Should be hidden");
            }
        }

        private async void Webview_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            logToConsole("WARNING: Do not use random code snippets! Make sure you can read the code, otherwise you're risking your account being stolen!"); // Dunno why people paste random code snippets into the console, well. I mean, some are good, but still.
            logToConsole("Started loading");

            // Give discord time to load, this is 4 seconds
            await Task.Delay(4000);
            // Guess we need to do it here, how annoying
            await updateChecker(sender, e); // you know what. im giving up on this. omg this was so annoying... actually imma try having the notification on a seperate window
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
        /// <returns><see cref="Task{string}"/>, which I think represents the javascript code which was run</returns>
        public async static Task<string> injectCSS(this WebView2 theView, string css)
        {
            // Why not make this built-in? I don't like having to use an extension method for something that should be built-in but whatever I guess
            return await theView.ExecuteScriptAsync("var styleSheet = document.createElement(\"style\"); styleSheet.innerText = " + css + "; document.head.appendChild(styleSheet);");
        }
    }
}
