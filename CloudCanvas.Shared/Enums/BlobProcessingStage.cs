using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Enums
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
