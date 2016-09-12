using System.Collections;
using System.Collections.Generic;

namespace OfficeSpace.Models.Dynamic
{
    public class ResultSetModel
    {
        public ResultSetModel()
        {
            this.Columns = new List<string>();
            this.Rows = new Dictionary<int, ArrayList>();
            this.DataSet = new List<IDictionary<string, object>>();
            this.ExpandoDataSet = new List<dynamic>();
        }

        /// <summary>
        /// Checks if the column names has been fetched.
        /// </summary>
        public bool IsColumnsFetched { get; set; }

        public List<string> Columns { get; set; }
        public IDictionary<int, ArrayList> Rows { get; set; }

        public List<IDictionary<string, object>> DataSet { get; set; }
        public List<dynamic> ExpandoDataSet { get; set; }
    }
}