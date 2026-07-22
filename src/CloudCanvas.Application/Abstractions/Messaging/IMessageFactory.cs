using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Abstractions.Messaging
{
    public interface IMessageFactory
    {
        IMessageBuilder BuildFor(object payload);
    }
}
