using CloudCanvas.Functions.Orchestration.DTO;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class InceptionRequest(BlobMetaDTO blob, string correlationId) : CorrelatedRequest(correlationId)
    {
        public BlobMetaDTO Blob { get; } = blob;
    }
}
