using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.IEntities
{
    public interface ISoftDeletable
    {
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }
    }
}
