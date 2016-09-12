using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeSpace.DataAccess.Models
{
    public class Unit : EntityBase
    {
        public Unit()
        {
            this.Workspaces = new HashSet<Workspace>();
        }

        public string Name { get; set; }
        public string Code { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int FloorId { get; set; }
        public virtual Floor Floor { get; set; }

        public virtual ICollection<Workspace> Workspaces { get; set; }
    }
}
