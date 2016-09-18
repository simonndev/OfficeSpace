using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeSpace.DataAccess;
using OfficeSpace.DataAccess.Models;
using OfficeSpace.Models.Dynamic;
using OfficeSpace.Models.Master;
using OfficeSpace.Models.SlickGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OfficeSpace.Controllers
{
    public class MasterController : Controller
    {
        private OfficeSpaceContext _dbContext;

        public MasterController(OfficeSpaceContext dbContext)
        {
            this._dbContext = dbContext;
        }

        // GET: Master
        public async Task<ActionResult> Index()
        {
            var locationSelection = await _dbContext.Locations
                .Where(l => !l.IsDeleted)
                .Select(l => new SelectListItem
                {
                    Text = l.Name,
                    Value = l.Id.ToString()
                })
                .ToListAsync();

            locationSelection.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            
            ViewData["LocationSelection"] = locationSelection;

            return View();
        }

        // GET: Master/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Master/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Master/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Master/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Master/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Master/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Master/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> GetTreeGridJson(int locationId = 0, int? buildingId = null, int? floorId = null, int? unitId = null)
        {
            var slickgrid = await PopulateMasterTreeGrid(locationId, buildingId, floorId, unitId);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string json = JsonConvert.SerializeObject(
                new {
                    Columns = slickgrid.GridColumns,
                    Data = slickgrid.TreeData
                },
                Formatting.Indented,
                settings);

            return Content(json, "application/json", System.Text.Encoding.UTF8);
        }

        private async Task<SlickGridDynamicModel> PopulateMasterTreeGrid(
            int locationId = 0,
            int? buildingId = null, int? floorId = null, int? unitId = null)
        {
            SlickGridDynamicModel slickgrid = new SlickGridDynamicModel();

            var tree = new List<dynamic>();
            int id = 0;
            int parentId = 0;

            if (locationId == 0)
            {
                /* 
                 * iterate through all Locations
                 * 
                 * WARNING: Beware of the "There is already an open DataReader associated with this Command which must be closed first." error.
                 * Try to use ToList() at the end of every query; or
                 * Add MultipleActiveResultSets=true to the provider part of your connection string.
                 */
                var locations = await _dbContext.Locations.Where(l => !l.IsDeleted).ToListAsync();
                if (locations.Count > 0)
                {
                    var result = await CountAllTypeOfWorkspaceIn(true, locationId);

                    foreach (var location in locations)
                    {
                        dynamic locationModel = new ExpandoObject();
                        locationModel.Id = "id_" + id.ToString();
                        locationModel.Indent = 0;
                        locationModel.Parent = null;

                        locationModel.ItemId = location.Id;
                        locationModel.Name = location.Name;
                        locationModel.Code = location.Code;
                        locationModel.CreatedDate = location.CreatedDate.ToString("dd-MMM-yyyy hh:mm");
                        locationModel.UpdatedDate = location.UpdatedDate.ToString("dd-MMM-yyyy hh:mm");

                        dynamic locationExpando = result.ExpandoDataSet.SingleOrDefault(e => e.ItemId == location.Id);
                        if (locationExpando == null)
                        {
                            locationExpando = result.GetDefaultExpando(location.Id, location.Name);

                            dynamic treeGridItem = Combine(locationExpando, locationModel);

                            var gridColumnInfos = (ICollection<KeyValuePair<string, object>>)treeGridItem;
                            slickgrid.GridColumns = gridColumnInfos.Select(kvp => new SlickGridColumnModel
                            {
                                Id = "col" + kvp.Key,
                                Name = kvp.Key,
                                Field = Char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1)
                            }).ToList();

                            tree.Add(treeGridItem);
                        }
                        else
                        {
                            dynamic treeGridItem = Combine(locationExpando, locationModel);

                            var gridColumnInfos = (ICollection<KeyValuePair<string, object>>)treeGridItem;
                            slickgrid.GridColumns = gridColumnInfos.Select(kvp => new SlickGridColumnModel
                            {
                                Id = "col" + kvp.Key,
                                Name = kvp.Key,
                                Field = Char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1)
                            }).ToList();

                            tree.Add(treeGridItem);

                            var locationBranch = await PopulateLocationBranch(
                                itemId => id = itemId, () => id,
                                pId => parentId = pId, () => parentId,
                                location.Id);

                            tree.AddRange(locationBranch);
                        }

                        ++id;
                    }
                }
            }
            else
            {
                // locationId > 0 (it shouldn't be NULL, should it?)
                var location = await _dbContext.Locations.SingleOrDefaultAsync(l => l.Id == locationId);
                dynamic locationModel = new ExpandoObject();
                locationModel.Id = "id_" + id.ToString();
                locationModel.Indent = 0;
                locationModel.Parent = null;

                locationModel.ItemId = location.Id;
                locationModel.Name = location.Name;
                locationModel.Code = location.Code;
                locationModel.CreatedDate = location.CreatedDate.ToString("dd-MMM-yyyy hh:mm");
                locationModel.UpdatedDate = location.UpdatedDate.ToString("dd-MMM-yyyy hh:mm");

                var result = await CountAllTypeOfWorkspaceIn(true, locationId);
                dynamic locationExpando = result.ExpandoDataSet.SingleOrDefault(e => e.ItemId == location.Id);

                dynamic treeGridItem = Combine(locationExpando, locationModel);

                var gridColumnInfos = (ICollection<KeyValuePair<string, object>>)treeGridItem;
                slickgrid.GridColumns = gridColumnInfos.Select(kvp => new SlickGridColumnModel
                {
                    Id = "col" + kvp.Key,
                    Name = kvp.Key,
                    Field = Char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1)
                }).ToList();

                tree.Add(treeGridItem);

                var locationBranch = await PopulateLocationBranch(
                    itemId => id = itemId, () => id,
                    pId => parentId = pId, () => parentId,
                    locationId,
                    buildingId, floorId, unitId);

                tree.AddRange(locationBranch);
            }

            slickgrid.TreeData = tree;

            return slickgrid;
        }

        /// <summary>
        /// Populate all the Buildings of a specific Location-ID.
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="buildingId"></param>
        /// <param name="floorId"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        private async Task<IEnumerable<dynamic>> PopulateLocationBranch(
            Action<int> setId, Func<int> getId,
            Action<int> setParentId, Func<int> getParentId,
            int locationId,
            int? buildingId = null, int? floorId = null, int? unitId = null)
        {
            var locationBranch = new List<dynamic>();
            int id = getId();

            if (!buildingId.HasValue || buildingId.Value == 0)
            {
                var buildings = await _dbContext.Buildings.Where(b => !b.IsDeleted && b.LocationId == locationId).ToListAsync();
                if (buildings.Count > 0)
                {
                    setParentId(id); // parentId = id

                    var result = await CountAllTypeOfWorkspaceIn(false, locationId, 0); // counts all Buildings
                    
                    foreach (var building in buildings)
                    {
                        dynamic buildingModel = new ExpandoObject();
                        buildingModel.Id = "id_" + (++id).ToString();
                        buildingModel.Indent = 1;
                        buildingModel.Parent = getParentId();

                        buildingModel.ItemId = building.Id;
                        buildingModel.Name = building.Name;
                        buildingModel.Code = building.Code;
                        buildingModel.CreatedDate = building.CreatedDate.ToString("dd-MMM-yyyy hh:mm");
                        buildingModel.UpdatedDate = building.UpdatedDate.ToString("dd-MMM-yyyy hh:mm");

                        dynamic buildingExpando = result.ExpandoDataSet.SingleOrDefault(e => e.ItemId == building.Id);
                        if (buildingExpando == null)
                        {
                            buildingExpando = result.GetDefaultExpando(building.Id, building.Name);

                            dynamic treeGridItem = Combine(buildingExpando, buildingModel);

                            locationBranch.Add(treeGridItem);

                            setId(id); // updates ID increment
                        }
                        else
                        {
                            dynamic treeGridItem = Combine(buildingExpando, buildingModel);

                            locationBranch.Add(treeGridItem);

                            setId(id); // updates ID increment

                            var buildingBranch = await PopulateBuildingBranch(
                                setId, getId,
                                setParentId, getParentId,
                                locationId,
                                building.Id);

                            locationBranch.AddRange(buildingBranch);

                            // makes sure the ID is continuous after populating the children.
                            id = getId();
                        }
                    }
                }
            }
            else
            {
                // locationId > 0 && buildingId > 0 (it shouldn't be NULL, should it?)
                var building = await _dbContext.Buildings.SingleOrDefaultAsync(b => b.Id == buildingId.Value);
                dynamic buildingModel = new ExpandoObject();
                buildingModel.Id = "id_" + (++id).ToString();
                buildingModel.Indent = 1;
                buildingModel.Parent = getParentId();

                buildingModel.ItemId = building.Id;
                buildingModel.Name = building.Name;
                buildingModel.Code = building.Code;
                buildingModel.CreatedDate = building.CreatedDate.ToString("dd-MMM-yyyy hh:mm");
                buildingModel.UpdatedDate = building.UpdatedDate.ToString("dd-MMM-yyyy hh:mm");

                var result = await CountAllTypeOfWorkspaceIn(false, locationId, buildingId.Value);
                dynamic buildingExpando = result.ExpandoDataSet.SingleOrDefault(e => e.ItemId == buildingId.Value);

                dynamic treeGridItem = Combine(buildingExpando, buildingModel);

                setParentId(id); // parentId = id

                locationBranch.Add(treeGridItem);

                setId(id); // updates ID increment

                var buildingBranch = await PopulateBuildingBranch(
                    setId, getId,
                    setParentId, getParentId,
                    locationId,
                    buildingId.Value,
                    floorId, unitId);

                locationBranch.AddRange(buildingBranch);

                // makes sure the ID is continuous after populating the children.
                id = getId();
            }

            return locationBranch;
        }

        private async Task<IEnumerable<dynamic>> PopulateBuildingBranch(
            Action<int> setId, Func<int> getId,
            Action<int> setParentId, Func<int> getParentId,
            int locationId, int buildingId,
            int? floorId = null, int? unitId = null)
        {
            var buildingBranch = new List<dynamic>();
            int id = getId();

            if (!floorId.HasValue || floorId.Value == 0)
            {
                var floors = await _dbContext.Floors.Where(f => !f.IsDeleted && f.BuildingId == buildingId).ToListAsync();
                if (floors.Count > 0)
                {
                    setParentId(id); // parentId = id

                    var result = await CountAllTypeOfWorkspaceIn(false, locationId, buildingId, 0); // counts all Floors

                    foreach (var floor in floors)
                    {
                        dynamic floorModel = new ExpandoObject();
                        floorModel.Id = "id_" + (++id).ToString();
                        floorModel.Indent = 2;
                        floorModel.Parent = getParentId();

                        floorModel.ItemId = floor.Id;
                        floorModel.Name = floor.Name;
                        floorModel.Code = floor.Code;
                        floorModel.CreatedDate = floor.CreatedDate.ToString("dd-MMM-yyyy hh:mm");
                        floorModel.UpdatedDate = floor.UpdatedDate.ToString("dd-MMM-yyyy hh:mm");

                        dynamic floorExpando = result.ExpandoDataSet.SingleOrDefault(e => e.ItemId == floor.Id);
                        if (floorExpando == null)
                        {
                            floorExpando = result.GetDefaultExpando(floor.Id, floor.Name);

                            dynamic treeGridItem = Combine(floorExpando, floorModel);

                            buildingBranch.Add(treeGridItem);

                            setId(id); // updates ID increment
                        }
                        else
                        {
                            dynamic treeGridItem = Combine(floorExpando, floorModel);

                            buildingBranch.Add(treeGridItem);
                            setId(id); // updates ID increment

                            var floorBranch = await PopulateFloorBranch(
                                setId, getId,
                                setParentId, getParentId,
                                locationId, buildingId, floor.Id);

                            buildingBranch.AddRange(floorBranch);

                            // makes sure the ID is continuous after populating the children.
                            id = getId();
                        }
                    }
                }
            }
            else
            {
                // locationId > 0 && buildingId > 0 && floorId > 0 (it shouldn't be NULL, should it?)
                var floor = await _dbContext.Floors.SingleOrDefaultAsync(f => f.Id == floorId.Value);
                dynamic floorModel = new ExpandoObject();
                floorModel.Id = "id_" + (++id).ToString();
                floorModel.Indent = 2;
                floorModel.Parent = getParentId();

                floorModel.ItemId = floor.Id;
                floorModel.Name = floor.Name;
                floorModel.Code = floor.Code;
                floorModel.CreatedDate = floor.CreatedDate.ToString("dd-MMM-yyyy hh:mm");
                floorModel.UpdatedDate = floor.UpdatedDate.ToString("dd-MMM-yyyy hh:mm");

                var result = await CountAllTypeOfWorkspaceIn(false, locationId, buildingId, floorId.Value);
                dynamic floorExpando = result.ExpandoDataSet.SingleOrDefault(e => e.ItemId == floorId.Value);

                dynamic treeGridItem = Combine(floorExpando, floorModel);

                setParentId(id);

                buildingBranch.Add(treeGridItem);

                setId(id); // updates ID increment

                var floorBranch = await PopulateFloorBranch(
                    setId, getId,
                    setParentId, getParentId,
                    locationId, buildingId, floorId.Value,
                    unitId);

                buildingBranch.AddRange(floorBranch);

                // makes sure the ID is continuous after populating the children.
                id = getId();
            }

            return buildingBranch;
        }

        private async Task<IEnumerable<dynamic>> PopulateFloorBranch(
            Action<int> setId, Func<int> getId,
            Action<int> setParentId, Func<int> getParentId,
            int locationId, int buildingId, int floorId,
            int? unitId = null)
        {
            var floorBranch = new List<dynamic>();
            int id = getId();

            if (!unitId.HasValue || unitId.Value == 0)
            {
                var units = await _dbContext.Units.Where(u => !u.IsDeleted && u.FloorId == floorId).ToListAsync();
                if (units.Count > 0)
                {
                    setParentId(id); // parentId = id

                    var result = await CountAllTypeOfWorkspaceIn(false, locationId, buildingId, floorId, 0); // counts all Units

                    foreach (var unit in units)
                    {
                        dynamic unitModel = new ExpandoObject();
                        unitModel.Id = "id_" + (++id).ToString();
                        unitModel.Indent = 3;
                        unitModel.Parent = getParentId();

                        unitModel.ItemId = unit.Id;
                        unitModel.Name = unit.Name;
                        unitModel.Code = unit.Code;
                        unitModel.CreatedDate = unit.CreatedDate.ToString("dd-MMM-yyyy hh:mm");
                        unitModel.UpdatedDate = unit.UpdatedDate.ToString("dd-MMM-yyyy hh:mm");

                        dynamic unitExpando = result.ExpandoDataSet.SingleOrDefault(e => e.ItemId == unit.Id);
                        if (unitExpando == null)
                        {
                            unitExpando = result.GetDefaultExpando(unit.Id, unit.Name);
                        }

                        dynamic treeGridItem = Combine(unitExpando, unitModel);

                        floorBranch.Add(treeGridItem);

                        setId(id); // updates ID increment
                    }
                }
            }
            else
            {
                setParentId(id);

                // locationId > 0 && buildingId > 0 && floorId > 0 && unitId > 0
                var unit = await _dbContext.Units.FindAsync(unitId.Value); // unit shouldn't be NULL, should it?
                dynamic unitModel = new ExpandoObject();
                unitModel.Id = "id_" + (++id).ToString();
                unitModel.Indent = 3;
                unitModel.Parent = getParentId();

                unitModel.ItemId = unit.Id;
                unitModel.Name = unit.Name;
                unitModel.Code = unit.Code;
                unitModel.CreatedDate = unit.CreatedDate.ToString("dd-MMM-yyyy hh:mm");
                unitModel.UpdatedDate = unit.UpdatedDate.ToString("dd-MMM-yyyy hh:mm");

                var result = await CountAllTypeOfWorkspaceIn(false, locationId, buildingId, floorId, unitId.Value);
                dynamic unitExpando = result.ExpandoDataSet.SingleOrDefault(e => e.ItemId == unit.Id);

                dynamic treeGridItem = Combine(unitExpando, unitModel);

                floorBranch.Add(treeGridItem);

                setId(id); // updates ID increment
            }

            return floorBranch;
        }
        
        private async Task<ResultSetModel> CountAllTypeOfWorkspaceIn(bool getColumnInfos = false, int locationId = 0, int? buildingId = null, int? floorId = null, int? unitId = null)
        {
            var resultSet = new ResultSetModel();
            string currentColumnName = string.Empty;

            IDictionary<int, ArrayList> row = null;
            int rowKey = 0;
            ArrayList rowData = null;

            // Create a SQL command to execute the sproc
            using (var command = _dbContext.Database.Connection.CreateCommand())
            {
                command.CommandText = "[dbo].[USP_CountAllTypeOfWorkspace]";
                command.CommandType = CommandType.StoredProcedure;

                DbParameter locationIdParam = command.CreateParameter();
                locationIdParam.ParameterName = "@LocationID";
                locationIdParam.DbType = DbType.Int32;
                locationIdParam.Direction = ParameterDirection.Input;
                locationIdParam.Value = locationId;

                DbParameter buildingIdParam = command.CreateParameter();
                //buildingIdParam.IsNullable = true;
                buildingIdParam.ParameterName = "@BuildingID";
                buildingIdParam.DbType = DbType.Int32;
                buildingIdParam.Direction = ParameterDirection.Input;
                if (buildingId.HasValue)
                {
                    buildingIdParam.Value = buildingId.Value;
                }
                else
                {
                    buildingIdParam.Value = DBNull.Value;
                }

                DbParameter floorIdParam = command.CreateParameter();
                //floorIdParam.IsNullable = true;
                floorIdParam.ParameterName = "@FloorID";
                floorIdParam.DbType = DbType.Int32;
                floorIdParam.Direction = ParameterDirection.Input;
                if (floorId.HasValue)
                {
                    floorIdParam.Value = floorId.Value;
                }
                else
                {
                    floorIdParam.Value = DBNull.Value;
                }

                DbParameter unitIdParam = command.CreateParameter();
                //unitIdParam.IsNullable = true;
                unitIdParam.ParameterName = "@UnitID";
                unitIdParam.DbType = DbType.Int32;
                unitIdParam.Direction = ParameterDirection.Input;
                if (unitId.HasValue)
                {
                    unitIdParam.Value = unitId.Value;
                }
                else
                {
                    unitIdParam.Value = DBNull.Value;
                }

                command.Parameters.Add(locationIdParam);
                command.Parameters.Add(buildingIdParam);
                command.Parameters.Add(floorIdParam);
                command.Parameters.Add(unitIdParam);

                try
                {
                    _dbContext.Database.Connection.Open();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.FieldCount > 0 && getColumnInfos)
                        {
                            // gets list of column names from the DbDataReader ONCE.
                            resultSet.Columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                            resultSet.ColumnInfos = Enumerable.Range(0, reader.FieldCount)
                                .ToDictionary(
                                    i => reader.GetName(i),
                                    i => reader.GetFieldType(i));
                        }

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                row = new Dictionary<int, ArrayList>();
                                rowData = new ArrayList();

                                // the first field is defined as ID integer
                                rowKey = reader.GetInt32(0);

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var cell = reader.GetValue(i);
                                    if (cell is DBNull)
                                    {
                                        cell = null;
                                    }

                                    rowData.Add(cell);
                                }

                                var set = Enumerable.Range(0, reader.FieldCount)
                                   .ToDictionary(
                                       i => reader.GetName(i),
                                       i => reader.GetValue(i));

                                var expando = new ExpandoObject();
                                var expandoSet = (ICollection<KeyValuePair<string, object>>)expando;
                                foreach (var kvp in set)
                                {
                                    expandoSet.Add(kvp);
                                }

                                dynamic dexpando = expandoSet;

                                resultSet.ExpandoDataSet.Add(expandoSet);

                                resultSet.Rows.Add(rowKey, rowData);
                            }
                        }
                    }
                }
                finally
                {
                    _dbContext.Database.Connection.Close();
                }
            }

            return resultSet;
        }

        private static dynamic Merge(object expandoItem1, object expandoItem2)
        {
            if (expandoItem1 == null || expandoItem2 == null)
                return expandoItem1 ?? expandoItem2 ?? new ExpandoObject();

            dynamic expando = new ExpandoObject();

            var result = expando as IDictionary<string, object>;
            foreach (System.Reflection.PropertyInfo fi in expandoItem1.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(expandoItem1, null);
            }
            foreach (System.Reflection.PropertyInfo fi in expandoItem2.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(expandoItem2, null);
            }

            return result;
        }

        private static dynamic Combine(dynamic item1, dynamic item2)
        {
            var dictionary1 = (IDictionary<string, object>)item1;
            var dictionary2 = (IDictionary<string, object>)item2;
            var result = new ExpandoObject();
            var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary

            foreach (var pair in dictionary1.Concat(dictionary2))
            {
                d[pair.Key] = pair.Value;
            }

            return result;
        }
    }
}
