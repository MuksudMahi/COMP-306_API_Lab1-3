using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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
    /// Interaction logic for ReadBook.xaml
    /// </summary>
    public partial class ReadBook : Window
    {
        private static AmazonS3Client s3Client = new AmazonS3Client(RegionEndpoint.USEast1);
        public static IAmazonDynamoDB client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        DynamoDBContext context = new DynamoDBContext(client);
        BookShelf bookShelf;

        public ReadBook(BookShelf selectedBook)
        {
            InitializeComponent();
            bookShelf = selectedBook;
            GetSelectedItem();
        }

        public async void GetSelectedItem()
        {
            MemoryStream content = await GetSelectedItemFromS3("bucket-lab-2");
            pdfReader.Load(content);
            pdfReader.CurrentPage = bookShelf.Bookmark;

        }

        private async Task<MemoryStream> GetSelectedItemFromS3(string bucketName)
        {
            GetObjectRequest request = new GetObjectRequest();
            request.BucketName = bucketName;
            request.Key = bookShelf.Key;
            GetObjectResponse resp = await s3Client.GetObjectAsync(request);
            MemoryStream documentStream = new MemoryStream();
            resp.ResponseStream.CopyTo(documentStream);
            return documentStream;
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bookShelf.Bookmark = pdfReader.CurrentPage;
            bookShelf.Accessed = DateTime.Now;
            await context.SaveAsync(bookShelf);
        }
    }
}
