using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.IEntities
{
    public interface ICreateable
    {
        public DateTime CreatedAt { get; set; }

    }
}
