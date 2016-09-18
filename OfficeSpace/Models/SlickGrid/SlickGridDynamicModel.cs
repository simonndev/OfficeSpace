using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeSpace.Models.SlickGrid
{
    public class SlickGridDynamicModel
    {
        public List<SlickGridColumnModel> GridColumns { get; set; }
        public List<dynamic> TreeData { get; set; }
    }
}