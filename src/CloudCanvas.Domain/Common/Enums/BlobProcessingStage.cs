using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Domain.Common.Enums
{
    public enum BlobProcessingStage
    {
        UploadSuccessful,
        ExtractMetadata,
        CreateThumbnail,
        UpdateMetadata,
        Intelligence,
        Completed
    }
}
