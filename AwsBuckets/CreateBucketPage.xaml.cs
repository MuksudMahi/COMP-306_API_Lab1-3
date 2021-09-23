using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Configuration;
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
    /// Interaction logic for CreateBucketPage.xaml
    /// </summary>
    /// 
    public partial class CreateBucketPage : Page
    {
        //MainPage mainPage;
        public static ObservableCollection<BucketModel> BucketList = new();
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
        private static AmazonS3Client s3Client;
        public CreateBucketPage()
        {
            InitializeComponent();
            getConnection();
            GetBucketList();
            dgBucketList.ItemsSource = BucketList;
        }

        private async void btnCreateBucket_Click(object sender, RoutedEventArgs e)
        {
            String name = tbBucketName.Text.ToString();
            await CreateBucketAsync(name);
        }

        private void getConnection()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);

            var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

            var credentials = new BasicAWSCredentials(accessKeyID, secretKey);
            s3Client = new AmazonS3Client(credentials, bucketRegion);
        }

        private static async void GetBucketList()
        {
            BucketList.Clear();
            ListBucketsResponse response = await s3Client.ListBucketsAsync();
            foreach (S3Bucket bucket in response.Buckets)
            {
                BucketList.Add(new BucketModel(bucket.BucketName, bucket.CreationDate));
            }
        }

        private async Task  CreateBucketAsync(string bucketName)
        {
            PutBucketResponse putBucketResponse = null;
            try
            {
                if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);
                    GetBucketList();
                }
                else if(await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName))
                {
                    tbMessage.Text = "Bucket Already Exist";

                }

            }
            catch (AmazonS3Exception e)
            {
                tbMessage.Text = e.Message.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        private void btnReturnToMain_Click(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = new MainPage();
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.Height = 300;
            mainWindow.Content = mainPage;
        }
    }
}
