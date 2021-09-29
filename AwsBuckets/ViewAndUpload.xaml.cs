using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace AwsBuckets
{
    /// <summary>
    /// Interaction logic for ViewAndUpload.xaml
    /// </summary>
    public partial class ViewAndUpload : Page
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
        private static AmazonS3Client s3Client;
        private static List<BucketModel> bucketsCB = new();
        ObservableCollection<ObjectModel> bucketObjects = new();
        static string bucketName;

        public ViewAndUpload()
        {
            InitializeComponent();
            s3Client = AWSConnection.getConnection();
            prepareComboBox();
        }

        private void prepareComboBox()
        {
            cbBuckets.ItemsSource = bucketsCB;
            GetBucketList();

        }

        private static async void GetBucketList()
        {
            ListBucketsResponse response = await s3Client.ListBucketsAsync();
            foreach (S3Bucket bucket in response.Buckets)
            {
                bucketsCB.Add(new BucketModel(bucket.BucketName, bucket.CreationDate));
            }
        }
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                tbFileLoc.Text = openFileDialog.FileName;
        }

        private void bntUpload_Click(object sender, RoutedEventArgs e)
        {
            uploadFileAsync();
            tbFileLoc.Text = "";
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = new MainPage();
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.Height = 300;
            mainWindow.Title = "Lab#1";
            mainWindow.Content = mainPage;
        }

        private void cbBuckets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bucketObjects.Clear();
            ListingObjects();
            //tbTest.Text = cbBuckets.SelectedItem.ToString();
        }
        private async void ListingObjects()
        {
            
            try
            {
                bucketName = cbBuckets.SelectedItem.ToString();
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = bucketName;
                ListObjectsResponse response = await s3Client.ListObjectsAsync(request);
                foreach (S3Object entry in response.S3Objects)
                {
                    //tbTest.Text = entry.Key;
                    bucketObjects.Add(new ObjectModel(entry.Key, entry.Size));


                }
                dgFiles.ItemsSource = bucketObjects;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when listing objects", amazonS3Exception.Message);
                }
            }
        }

        private async void uploadFileAsync()
        {
            string filePath = tbFileLoc.Text;
            if(filePath!="")
            {
                bucketName = cbBuckets.SelectedItem.ToString();
                try
                {
                    var fileTransferUtility = new TransferUtility(s3Client);
                    await fileTransferUtility.UploadAsync(filePath, bucketName);
                    ListingObjects();
                }
                catch (AmazonS3Exception e)
                {
                    tbTest.Text = e.Message.ToString();
                }
                catch (Exception e)
                {
                    tbTest.Text = e.Message.ToString();
                }
            }

        }
    }
}
