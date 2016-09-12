using System;

namespace OfficeSpace.DataAccess.Models
{
    public class Workspace : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }

        public int WorkspaceTypeId { get; set; }
        public virtual WorkspaceType WorkspaceType { get; set; }
    }
}
