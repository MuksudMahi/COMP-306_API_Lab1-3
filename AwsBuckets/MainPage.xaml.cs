using System;
using System.Collections.Generic;
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

namespace AwsBuckets
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        CreateBucketPage page;
        ViewAndUpload viewAndUpload;
        public MainPage()
        {
            InitializeComponent();
            viewAndUpload = new ViewAndUpload();
            page = new CreateBucketPage();


        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.Height = 450;
            mainWindow.Content = page;
        }

        private void btnObj_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.Height = 450;
            mainWindow.Content = viewAndUpload;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
