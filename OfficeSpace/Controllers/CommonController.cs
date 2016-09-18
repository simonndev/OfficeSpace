using OfficeSpace.DataAccess;
using OfficeSpace.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OfficeSpace.Controllers
{
    public class CommonController : Controller
    {
        private OfficeSpaceContext _dbContext;

        public CommonController(OfficeSpaceContext dbContext)
        {
            this._dbContext = dbContext;
        }

        // GET: Common
        public async Task<JsonResult> PopulateBuildingSelectionByLocation(int locationId = 0)
        {
            var buildings = await _dbContext.Buildings
                .Where(b => !b.IsDeleted && (locationId != 0 ? b.LocationId == locationId : true))
                .Select(b => new
                {
                    Id = b.Id,
                    Name = b.Name
                }).ToListAsync();

            buildings.Insert(0, new { Id = 0, Name = "All Buildings" });

            return Json(buildings, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> PopulateFloorSelectionByBuilding(int buildingId = 0)
        {
            var floors = await _dbContext.Floors
                .Where(f => !f.IsDeleted && (buildingId != 0 ? f.BuildingId == buildingId : true))
                .Select(f => new
                {
                    Id = f.Id,
                    Name = f.Name
                }).ToListAsync();

            floors.Insert(0, new { Id = 0, Name = "All Floors" });

            return Json(floors, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> PopulateUnitSelectionByFloor(int floorId = 0)
        {
            var units = await _dbContext.Units
                .Where(u => !u.IsDeleted && (floorId != 0 ? u.FloorId == floorId : true))
                .Select(u => new
                {
                    Id = u.Id,
                    Name = u.Name
                }).ToListAsync();

            units.Insert(0, new { Id = 0, Name = "All Units" });

            return Json(units, JsonRequestBehavior.AllowGet);
        }
    }
}