using OfficeSpace.BusinessLogic.Models;
using OfficeSpace.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeSpace.BusinessLogic.Services
{
    public interface IMasterService
    {
        Task<IEnumerable<object>> PopulateMasterTreeGridAsync(
            int locationId = 0,
            int? buildingId = null, int? floorId = null, int? unitId = null);
    }

    public class MasterService : ServiceBase, IMasterService
    {
        public MasterService(OfficeSpaceContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<object>> PopulateMasterTreeGridAsync(
            int locationId = 0,
            int? buildingId = default(int?), int? floorId = default(int?), int? unitId = default(int?))
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
                if (locations.Any())
                {
                    foreach (var location in locations)
                    {
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
                            location.Id);

                        tree.AddRange(locationBranch);

                        ++id;
                    }
                }
            }
            else
            {
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
                if (buildings.Any())
                {
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

                        setId(id);

                        var buildingBranch = await PopulateBuildingBranch(setId, getId, setParentId, getParentId, building.Id);

                        locationBranch.AddRange(buildingBranch);
                    }
                }
            }
            else
            {
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

                setId(id);

                var buildingBranch = await PopulateBuildingBranch(
                    setId, getId,
                    setParentId, getParentId,
                    buildingId.Value,
                    floorId, unitId);

                locationBranch.AddRange(buildingBranch);
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
                if (floors.Any())
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

                        setId(id);

                        var floorBranch = await PopulateFloorBranch(
                            setId, getId,
                            setParentId, getParentId,
                            floor.Id);

                        buildingBranch.AddRange(floorBranch);
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

                setId(id);

                var floorBranch = await PopulateFloorBranch(
                    setId, getId,
                    setParentId, getParentId,
                    floorId.Value,
                    unitId);

                buildingBranch.AddRange(floorBranch);
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
                if (units.Any())
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

                        setId(id);
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

                setId(id);
            }

            return floorBranch;
        }
    }
}