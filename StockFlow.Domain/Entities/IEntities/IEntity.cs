using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.IEntities
{
    public interface IEntity
    {
        public Guid Id { get; set; }
    }
}
