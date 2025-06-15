using Azure.Storage.Blobs.Models;
using CloudCanvas.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Interfaces
{
    public interface IBlobMetaConverter
    {
        public BlobMetaDTO ToBlobMeta(string blobUrl, BlobProperties blobProps);
        public string ToString(BlobMetaDTO blobMetaDTO);
    }
}
