using System;
using System.Collections.Generic;

namespace OfficeSpace.DataAccess.Models
{
    public class WorkspaceType : EntityBase
    {
        public WorkspaceType()
        {
            this.Workspaces = new HashSet<Workspace>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Workspace> Workspaces { get; set; }
    }
}
