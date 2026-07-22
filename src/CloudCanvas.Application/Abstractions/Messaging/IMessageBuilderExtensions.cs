using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Abstractions.Messaging
{
    public interface IMessageBuilderExtensions
    {
        IMessageBuilder CreateThumbnailsMessage(string correlationId);
    }
}
