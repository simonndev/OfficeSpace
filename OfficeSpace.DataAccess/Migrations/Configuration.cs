namespace OfficeSpace.DataAccess.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OfficeSpaceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(OfficeSpaceContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            #region Locations

            var locations = new List<Location>
            {
                new Location
                {
                    Name = "Location A",
                    Code = "LOC-A",
                    Description = null,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ParentId = null
                },
                new Location
                {
                    Name = "Location B",
                    Code = "LOC-B",
                    Description = null,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ParentId = null
                },
                new Location
                {
                    Name = "Location C",
                    Code = "LOC-C",
                    Description = null,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ParentId = null
                }
            };

            locations.ForEach(location => context.Locations.AddOrUpdate(l => l.Code, location));
            context.SaveChanges();

            var subLocations = new List<Location>
            {
                new Location
                {
                    Name = "Location A1",
                    Code = "SLOC-A1",
                    Description = null,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ParentId = 1
                },
                new Location
                {
                    Name = "Location A1",
                    Code = "SLOC-A2",
                    Description = null,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ParentId = 1
                },
                new Location
                {
                    Name = "Location B1",
                    Code = "SLOC-B1",
                    Description = null,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ParentId = 2
                },
                new Location
                {
                    Name = "Location C1",
                    Code = "SLOC-C1",
                    Description = null,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ParentId = 3
                }
            };

            subLocations.ForEach(subLocation => context.Locations.AddOrUpdate(l => l.Code, subLocation));
            context.SaveChanges();

            #endregion

            #region Buildings

            var buildings = new List<Building>
            {
                new Building
                {
                    Name = "Building 1",
                    Code = "LOC-A-B1",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    LocationId = 1
                },
                new Building
                {
                    Name = "Building 2",
                    Code = "SLOC-A1-B2",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    LocationId = 4
                },
                new Building
                {
                    Name = "Building 3",
                    Code = "LOC-C-B3",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    LocationId = 3
                }
            };

            buildings.ForEach(building => context.Buildings.AddOrUpdate(b => b.Code, building));
            context.SaveChanges();

            #endregion

            #region Floors

            GenerateFloorsForBuilding(context, 1, "LOC-A-B1-F", 5);
            GenerateFloorsForBuilding(context, 2, "SLOC-A1-B2-F", 3);
            GenerateFloorsForBuilding(context, 3, "LOC-C-B3-F", 7);

            #endregion

            #region Workspace-Types

            var workspaceTypes = new List<WorkspaceType>
            {
                new WorkspaceType
                {
                    Name = "Workstation",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                },
                new WorkspaceType
                {
                    Name = "Lab",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                },
                new WorkspaceType
                {
                    Name = "Meeting Room",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                }
            };

            workspaceTypes.ForEach(workspaceType => context.WorkspaceTypes.AddOrUpdate(wst => wst.Name, workspaceType));
            context.SaveChanges();

            #endregion
        }

        private int GenerateFloorsForBuilding(OfficeSpaceContext context, int buildingId, string codePrefix, int numberOfFloor = 1)
        {
            List<Floor> floors = new List<Floor>();

            Random floorNumberGenerator = new Random();
            for (int i = 1; i <= floorNumberGenerator.Next(1, numberOfFloor); i++)
            {
                floors.Add(new Floor
                {
                    Name = "Floor " + i.ToString(),
                    Code = codePrefix + i.ToString(),
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    BuildingId = buildingId
                });
            }

            floors.ForEach(floor => context.Floors.AddOrUpdate(f => f.Code, floor));
            return context.SaveChanges();
        }
    }
}
