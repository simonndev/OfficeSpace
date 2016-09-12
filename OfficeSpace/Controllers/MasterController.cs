using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeSpace.DataAccess;
using OfficeSpace.Models.Dynamic;
using OfficeSpace.Models.Master;
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
                }).ToListAsync();

            locationSelection.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = "0"
            });

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
            var tree = await PopulateMasterTreeGrid(locationId, buildingId, floorId, unitId);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string json = JsonConvert.SerializeObject(tree, Formatting.Indented, settings);

            return Content(json, "application/json", System.Text.Encoding.UTF8);
        }

        private async Task<IEnumerable<MasterTreeGridItemModel>> PopulateMasterTreeGrid(
            int locationId = 0,
            int? buildingId = null, int? floorId = null, int? unitId = null)
        {
            var tree = new List<MasterTreeGridItemModel>();
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
                    var result = await CountAllTypeOfWorkspace(locationId);

                    foreach (var location in locations)
                    {
                        if (result.Rows.ContainsKey(location.Id))
                        {
                            var rowData = result.Rows[location.Id];
                        }

                        tree.Add(new MasterTreeGridItemModel
                        {
                            Id = "id_" + id.ToString(),
                            Indent = 0,
                            Parent = null,

                            ItemId = location.Id,
                            Name = location.Name,
                            Code = location.Code,
                            CreatedDate = location.CreatedDate.ToString("dd-MMM-yyyy hh:mm"),
                            UpdatedDate = location.UpdatedDate.ToString("dd-MMM-yyyy hh:mm"),

                        });

                        var locationBranch = await PopulateLocationBranch(
                            itemId => id = itemId, () => id,
                            pId => parentId = pId, () => parentId,
                            location.Id);

                        tree.AddRange(locationBranch);

                        ++id;
                    }
                }
            }
            else
            {
                var result = await CountAllTypeOfWorkspace(locationId);

                // locationId > 0 (it shouldn't be NULL, should it?)
                var location = await _dbContext.Locations.SingleOrDefaultAsync(l => l.Id == locationId);

                tree.Add(new MasterTreeGridItemModel
                {
                    Id = "id_" + id.ToString(),
                    Indent = 0,
                    Parent = null,

                    ItemId = location.Id,
                    Name = location.Name,
                    Code = location.Code,
                    CreatedDate = location.CreatedDate.ToString("dd-MMM-yyyy hh:mm"),
                    UpdatedDate = location.UpdatedDate.ToString("dd-MMM-yyyy hh:mm")
                });

                var locationBranch = await PopulateLocationBranch(
                    itemId => id = itemId, () => id,
                    pId => parentId = pId, () => parentId,
                    locationId,
                    buildingId, floorId, unitId);

                tree.AddRange(locationBranch);
            }

            return tree;
        }

        /// <summary>
        /// Populate all the Buildings of a specific Location-ID.
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="buildingId"></param>
        /// <param name="floorId"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        private async Task<IEnumerable<MasterTreeGridItemModel>> PopulateLocationBranch(
            Action<int> setId, Func<int> getId,
            Action<int> setParentId, Func<int> getParentId,
            int locationId,
            int? buildingId = null, int? floorId = null, int? unitId = null)
        {
            var locationBranch = new List<MasterTreeGridItemModel>();
            int id = getId();
            int parent = getParentId();

            if (!buildingId.HasValue || buildingId.Value == 0)
            {
                var buildings = await _dbContext.Buildings.Where(b => !b.IsDeleted && b.LocationId == locationId).ToListAsync();
                if (buildings.Count > 0)
                {
                    var result = await CountAllTypeOfWorkspace(locationId, buildingId.Value);

                    setParentId(id); // parentId = id

                    foreach (var building in buildings)
                    {
                        locationBranch.Add(new MasterTreeGridItemModel
                        {
                            Id = "id_" + (++id).ToString(),
                            Indent = 1,
                            Parent = getParentId(),

                            ItemId = building.Id,
                            Name = building.Name,
                            Code = building.Code,
                            CreatedDate = building.CreatedDate.ToString("dd-MMM-yyyy hh:mm"),
                            UpdatedDate = building.UpdatedDate.ToString("dd-MMM-yyyy hh:mm")
                        });

                        setId(id); // updates ID increment

                        var buildingBranch = await PopulateBuildingBranch(setId, getId, setParentId, getParentId, building.Id);
                        
                        locationBranch.AddRange(buildingBranch);

                        // makes sure the ID is continuous after populating the children.
                        id = getId();
                    }
                }
            }
            else
            {
                var result = await CountAllTypeOfWorkspace(locationId, buildingId.Value);

                // locationId > 0 && buildingId > 0 (it shouldn't be NULL, should it?)
                var building = await _dbContext.Buildings.SingleOrDefaultAsync(b => b.Id == buildingId.Value);

                setParentId(id); // parentId = id

                locationBranch.Add(new MasterTreeGridItemModel
                {
                    Id = "id_" + (++id).ToString(),
                    Indent = 1,
                    Parent = getParentId(),

                    ItemId = building.Id,
                    Name = building.Name,
                    Code = building.Code,
                    CreatedDate = building.CreatedDate.ToString("dd-MMM-yyyy hh:mm"),
                    UpdatedDate = building.UpdatedDate.ToString("dd-MMM-yyyy hh:mm")
                });

                setId(id); // updates ID increment

                var buildingBranch = await PopulateBuildingBranch(
                    setId, getId,
                    setParentId, getParentId,
                    buildingId.Value,
                    floorId, unitId);

                locationBranch.AddRange(buildingBranch);

                // makes sure the ID is continuous after populating the children.
                id = getId();
            }

            return locationBranch;
        }

        private async Task<IEnumerable<MasterTreeGridItemModel>> PopulateBuildingBranch(
            Action<int> setId, Func<int> getId,
            Action<int> setParentId, Func<int> getParentId,
            int buildingId,
            int? floorId = null, int? unitId = null)
        {
            var buildingBranch = new List<MasterTreeGridItemModel>();
            int id = getId();

            if (!floorId.HasValue || floorId.Value == 0)
            {
                var floors = await _dbContext.Floors.Where(f => !f.IsDeleted && f.BuildingId == buildingId).ToListAsync();
                if (floors.Count > 0)
                {
                    setParentId(id); // parentId = id

                    foreach (var floor in floors)
                    {
                        buildingBranch.Add(new MasterTreeGridItemModel
                        {
                            Id = "id_" + (++id).ToString(),
                            Indent = 2,
                            Parent = getParentId(),

                            ItemId = floor.Id,
                            Name = floor.Name,
                            Code = floor.Code,
                            CreatedDate = floor.CreatedDate.ToString("dd-MMM-yyyy hh:mm"),
                            UpdatedDate = floor.UpdatedDate.ToString("dd-MMM-yyyy hh:mm")
                        });

                        setId(id); // updates ID increment

                        var floorBranch = await PopulateFloorBranch(
                            setId, getId,
                            setParentId, getParentId,
                            floor.Id);

                        buildingBranch.AddRange(floorBranch);

                        // makes sure the ID is continuous after populating the children.
                        id = getId();
                    }
                }
            }
            else
            {
                // locationId > 0 && buildingId > 0 && floorId > 0 (it shouldn't be NULL, should it?)
                var floor = await _dbContext.Buildings.SingleOrDefaultAsync(f => f.Id == floorId.Value);

                setParentId(id);

                buildingBranch.Add(new MasterTreeGridItemModel
                {
                    Id = "id_" + (++id).ToString(),
                    Indent = 2,
                    Parent = getParentId(),

                    ItemId = floor.Id,
                    Name = floor.Name,
                    Code = floor.Code,
                    CreatedDate = floor.CreatedDate.ToString("dd-MMM-yyyy hh:mm"),
                    UpdatedDate = floor.UpdatedDate.ToString("dd-MMM-yyyy hh:mm")
                });

                setId(id); // updates ID increment

                var floorBranch = await PopulateFloorBranch(
                    setId, getId,
                    setParentId, getParentId,
                    floorId.Value,
                    unitId);

                buildingBranch.AddRange(floorBranch);

                // makes sure the ID is continuous after populating the children.
                id = getId();
            }

            return buildingBranch;
        }

        private async Task<IEnumerable<MasterTreeGridItemModel>> PopulateFloorBranch(
            Action<int> setId, Func<int> getId,
            Action<int> setParentId, Func<int> getParentId,
            int floorId,
            int? unitId = null)
        {
            var floorBranch = new List<MasterTreeGridItemModel>();
            int id = getId();

            if (!unitId.HasValue || unitId.Value == 0)
            {
                var units = await _dbContext.Units.Where(u => !u.IsDeleted && u.FloorId == floorId).ToListAsync();
                if (units.Count > 0)
                {
                    setParentId(id); // parentId = id

                    foreach (var unit in units)
                    {
                        floorBranch.Add(new MasterTreeGridItemModel
                        {
                            Id = "id_" + (++id).ToString(),
                            Indent = 3,
                            Parent = getParentId(),

                            ItemId = unit.Id,
                            Name = unit.Name,
                            Code = unit.Code,
                            CreatedDate = unit.CreatedDate.ToString("dd-MMM-yyyy hh:mm"),
                            UpdatedDate = unit.UpdatedDate.ToString("dd-MMM-yyyy hh:mm")
                        });

                        setId(id); // updates ID increment
                    }
                }
            }
            else
            {
                // locationId > 0 && buildingId > 0 && floorId > 0
                var unit = await _dbContext.Buildings.FindAsync(unitId.Value); // unit shouldn't be NULL, should it?

                setParentId(id);

                floorBranch.Add(new MasterTreeGridItemModel
                {
                    Id = "id_" + (++id).ToString(),
                    Indent = 3,
                    Parent = getParentId(),

                    ItemId = unit.Id,
                    Name = unit.Name,
                    Code = unit.Code,
                    CreatedDate = unit.CreatedDate.ToString("dd-MMM-yyyy hh:mm"),
                    UpdatedDate = unit.UpdatedDate.ToString("dd-MMM-yyyy hh:mm")
                });

                setId(id); // updates ID increment
            }

            return floorBranch;
        }
        
        private async Task<ResultSetModel> CountAllTypeOfWorkspace(int locationId = 0, int? buildingId = null, int? floorId = null, int? unitId = null)
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
                        if (reader.FieldCount > 0)
                        {
                            if (!resultSet.IsColumnsFetched)
                            {
                                // gets list of column names from the DbDataReader ONCE.
                                resultSet.Columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                                resultSet.IsColumnsFetched = true;
                            }

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

                                resultSet.ExpandoDataSet.Add(expando);

                                resultSet.DataSet.Add(set);

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
    }
}
