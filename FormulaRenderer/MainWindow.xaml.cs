using System;
using System.Reflection;
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
using System.Text.RegularExpressions;

namespace FormulaRenderer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        async void webView_Loaded(object sender, EventArgs e)
        {
            var assem = Assembly.GetExecutingAssembly();

            var indexStream = assem.GetManifestResourceStream(assem.GetName().Name + ".index.html");
            if (indexStream != null)
            {
                using (var reader = new StreamReader(indexStream))
                {
                    await webView.EnsureCoreWebView2Async();
                    webView.NavigateToString(reader.ReadToEnd());
                }
            }
            else
            {
                throw new ArgumentNullException("Basic index file should be existed");
            }
        }

        private async void convertButton_Click(object sender, RoutedEventArgs e)
        {
            var renderStr = $"katex.render(\"{inputBox.Text.Replace(@"\", @"\\")}\", document.getElementById(\"render_div\"), {{ throwOnError: false }});";

            await webView.ExecuteScriptAsync(renderStr);
        }
    }
}
