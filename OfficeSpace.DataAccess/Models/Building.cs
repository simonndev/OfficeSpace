using System;
using System.Collections.Generic;

namespace OfficeSpace.DataAccess.Models
{
    public class Building : EntityBase
    {
        public Building()
        {
            this.Floors = new HashSet<Floor>();
        }

        public string Name { get; set; }
        public string Code { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public virtual ICollection<Floor> Floors { get; set; }
    }
}
