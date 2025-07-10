using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Exceptions
{
    [Serializable]
    public class BlobMetadataSerializationException: Exception
    {
        public BlobMetadataSerializationException(string message): base(message) { }
        public BlobMetadataSerializationException() { }
        public BlobMetadataSerializationException(string message, Exception innerException): base(message, innerException) { }
    }
}
