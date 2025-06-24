using Azure.Storage.Blobs.Models;
using CloudCanvas.Functions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Functions.Interfaces
{
    public interface IBlobMetaConverter
    {
        public BlobMetaDTO ToBlobMeta(string blobUrl, BlobProperties blobProps);
        public string ToString(CloudCanvasMessageDTO blobMetaDTO);
    }
}
