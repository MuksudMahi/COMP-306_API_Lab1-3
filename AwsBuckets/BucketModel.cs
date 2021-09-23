using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsBuckets
{
    public class BucketModel
    {
        public string bucketName { get; set; }
        public DateTime bucketCreationDate { get; set; }

        public BucketModel(string bucketName, DateTime bucketCreationDate)
        {
            this.bucketName = bucketName;
            this.bucketCreationDate = bucketCreationDate;
        }

        public override string ToString()
        {
            return $"{bucketName}";
        }
    }
}
