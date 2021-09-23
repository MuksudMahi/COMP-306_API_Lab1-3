using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsBuckets
{
    class ObjectModel
    {
        public string Name { get; set; }
        public long Size { get; set; }

        public ObjectModel(string name, long size)
        {
            Name = name;
            Size = size;
        }
    }
}
