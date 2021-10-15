using System.Threading;
using System.Windows;


namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            CreateTable.createTable();
            //Thread.Sleep(5000);
            InitializeComponent();
            AddUsers.RunDataModelSample();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string userName = tbUserName.Text;
            string password = tbPassword.Password;
            User user=null;

            if(userName!="" && password!="")
            {
                user = UserLogin.getUser(userName, password);
            }
            if(user!=null && user.Name==userName && user.Password==password)
            {
                BookList bookList = new BookList(userName);
                bookList.Show();
                this.Close();
            }
            else
            {
                tbMessage.Text = "Wrong username/password";
            }
        }
    }
}
