using OfficeSpace.DataAccess.Configurations;
using OfficeSpace.DataAccess.Models;
using System.Data.Entity;

namespace OfficeSpace.DataAccess
{
    public class OfficeSpaceContext : DbContext
    {
        public OfficeSpaceContext()
            : base("OfficeSpace")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceType> WorkspaceTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LocationConfiguration());
            modelBuilder.Configurations.Add(new BuildingConfiguration());
            modelBuilder.Configurations.Add(new FloorConfiguration());
            modelBuilder.Configurations.Add(new UnitConfiguration());
            modelBuilder.Configurations.Add(new WorkspaceConfiguration());
            modelBuilder.Configurations.Add(new WorkspaceTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
