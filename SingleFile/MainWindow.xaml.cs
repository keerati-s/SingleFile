using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SingleFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isProcessing = false;

        public MainWindow(string[] args)
        {
            InitializeComponent();
            this.Loaded += async (s, e) => await OnWindowLoaded(args);
            this.Closing += (s, e) => MainWindow_Closing(s, e);


        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_isProcessing)
            {
                MessageBoxResult result = MessageBox.Show(
                    "A process is currently running. Do you really want to exit?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true; // Cancel the closing event
                }
            }
        }

        private async Task OnWindowLoaded(string[] args)
        {
            if (args.Length == 3)
            {
                string SourcePath = args[0];
                string DestinationPath = args[1];
                string filePath = args[2];

                _isProcessing = true;

                ParamTextBlock.Text = $"Copying File from {SourcePath} to {DestinationPath} ";

                if (!File.Exists(filePath))
                {
                    _isProcessing = false;
                    Close();
                }
                else if (IsFileLocked(filePath))
                {
                    var result = MessageBox.Show($"The file '{filePath}' is in use and cannot be deleted.", "File In Use");
                    if (result == MessageBoxResult.OK)
                    {
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    File.Delete(filePath);
                    ParamTextBlock.Text = "Success";
                    await Task.Delay(50000);
                    Application.Current.Shutdown();
                }
                _isProcessing = false;
                /*await Dispatcher.BeginInvoke(new Action(() =>
                {

                    
                }), DispatcherPriority.Background);*/

            }
            else
            {
                _isProcessing = false;
                Close();
            }
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}