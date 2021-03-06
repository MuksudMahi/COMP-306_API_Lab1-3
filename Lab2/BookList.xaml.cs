using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3;
using Amazon.S3.Model;
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
using System.Windows.Shapes;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for BookList.xaml
    /// </summary>
    public partial class BookList : Window
    {
        public static IAmazonDynamoDB client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        DynamoDBContext context = new DynamoDBContext(client);
        private static AmazonS3Client s3Client = new AmazonS3Client(RegionEndpoint.USEast1);

        string UserName;
        List<BookShelf> bookLists;
        Dictionary<string, BookShelf> buttonMapping = new Dictionary<string, BookShelf>();

        public BookList(String userName)
        {
            InitializeComponent();
            this.UserName = userName;
            doMyWorks();
        }

        private async void doMyWorks()
        {
            bookLists = await GetBooks(UserName);
            int rows = 0;
            tbWelcome.Text += " " + UserName;
            foreach (BookShelf book in bookLists)
            {
                //RowDefinition gridRow1 = new RowDefinition();
                //gridRow1.Height = new GridLength(50);
                //bookGrid.RowDefinitions.Add(gridRow1);

                //RowDefinition gridRow2 = new RowDefinition();
                //gridRow2.Height = new GridLength(10);
                //bookGrid.RowDefinitions.Add(gridRow2);

                StackPanel stackPanel = new StackPanel();
                TextBlock textBlock = new TextBlock();
                Run bookName = new Run();
                Run author = new Run();
                Button button = new Button();

                bookName.FontWeight = FontWeights.Bold;
                bookName.FontSize = 20;

                button.Name = "btnBook" + rows.ToString();
                buttonMapping.Add(button.Name, book);

                bookName.Text = book.BookName;
                author.Text = book.Author;

                textBlock.Inlines.Add(bookName);
                textBlock.Inlines.Add(new LineBreak());
                textBlock.Inlines.Add(author);


                button.HorizontalContentAlignment = HorizontalAlignment.Left;
                button.Height = 50;
                button.Content = textBlock;
                button.Click += Button_Click;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.Children.Add(button);



                bookGrid.Children.Add(stackPanel);
                //Grid.SetRow(stackPanel, rows);
                rows+=rows+2;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            BookShelf selectedBook = buttonMapping[button.Name];
            ReadBook readBook = new ReadBook(selectedBook);
            this.Close();
            readBook.Show();
        }

        public async Task<List<BookShelf>> GetBooks(string UserName)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("UserName", ScanOperator.Equal, UserName));
            var books = await context.ScanAsync<BookShelf>(scanConditions).GetRemainingAsync();
            var bookList = books.OrderByDescending(book => Convert.ToDateTime(book.Accessed)).ToList();
            return bookList;
        }
    }
}
