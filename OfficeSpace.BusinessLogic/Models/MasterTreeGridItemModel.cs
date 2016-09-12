using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeSpace.BusinessLogic.Models
{
    public class MasterTreeGridItemModel
    {
        public string Id { get; set; }
        public int Indent { get; set; }
        public int? Parent { get; set; }

        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}
