using System;
using System.Collections.Generic;

namespace OfficeSpace.DataAccess.Models
{
    public class Location : EntityBase
    {
        public Location()
        {
            this.Buildings = new HashSet<Building>();
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int? ParentId { get; set; }
        public virtual Location Parent { get; set; }

        public virtual ICollection<Location> SubLocations { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }
    }
}
