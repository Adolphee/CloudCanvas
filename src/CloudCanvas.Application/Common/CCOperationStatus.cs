using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Common
{
    public enum CCOperationStatus
    {
        Success, Failed, NotFound, Unauthorized, InvalidInput, Conflict, ServerError
    }
}
