using System;
using System.Collections.Generic;

namespace OfficeSpace.DataAccess.Models
{
    public class Floor : EntityBase
    {
        public Floor()
        {
            this.Units = new HashSet<Unit>();
        }

        public string Name { get; set; }
        public string Code { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int BuildingId { get; set; }
        public virtual Building Building { get; set; }

        public virtual ICollection<Unit> Units { get; set; }
    }
}
