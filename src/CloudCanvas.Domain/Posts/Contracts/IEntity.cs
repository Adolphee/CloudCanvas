using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IEntity
    {
        public abstract string Id { get; init;}        
    }
}