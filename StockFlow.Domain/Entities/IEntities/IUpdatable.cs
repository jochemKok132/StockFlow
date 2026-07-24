using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.IEntities
{
    public interface IUpdatable
    {
        public DateTime UpdatedAt { get; set; }
    }
}
