using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace OfficeSpace.Models.Dynamic
{
    public class ResultSetModel
    {
        public ResultSetModel()
        {
            this.Columns = new List<string>();
            this.ColumnInfos = new Dictionary<string, Type>();

            this.Rows = new Dictionary<int, ArrayList>();
            this.DataSet = new List<IDictionary<string, object>>();
            this.ExpandoDataSet = new List<dynamic>();
        }

        public List<string> Columns { get; set; }
        public IDictionary<string, Type> ColumnInfos { get; set; }
        public IDictionary<int, ArrayList> Rows { get; set; }

        public List<IDictionary<string, object>> DataSet { get; set; }
        public List<dynamic> ExpandoDataSet { get; set; }

        public dynamic GetDefaultExpando(int id, string name)
        {
            var expando = new ExpandoObject();
            var expandoSet = (ICollection<KeyValuePair<string, object>>)expando;

            if (this.ColumnInfos.Count > 0)
            {
                foreach (var kvp in this.ColumnInfos)
                {
                    if (kvp.Key == "ItemId")
                    {
                        expandoSet.Add(new KeyValuePair<string, object>("ItemId", id));
                        continue;
                    }

                    if (kvp.Key == "Name")
                    {
                        expandoSet.Add(new KeyValuePair<string, object>("Name", name));
                        continue;
                    }

                    expandoSet.Add(new KeyValuePair<string, object>(kvp.Key, Activator.CreateInstance(kvp.Value)));
                }
            }

            dynamic expandoResult = expandoSet;

            return expandoResult;
        }
    }
}