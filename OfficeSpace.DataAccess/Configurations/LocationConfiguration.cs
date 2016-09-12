using OfficeSpace.DataAccess.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace OfficeSpace.DataAccess.Configurations
{
    public class LocationConfiguration : EntityTypeConfiguration<Location>
    {
        public LocationConfiguration()
        {
            this.HasKey(l => l.Id);
            this.Property(l => l.Id)
                .HasColumnName("LocationId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(l => l.Name)
                .HasColumnName("LocationName")
                .IsRequired()
                .HasMaxLength(255);

            this.Property(l => l.Code)
                .HasColumnName("LocationCode")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("Idx_Location_Code", 1)
                        { IsUnique = true }));

            this.HasOptional(l => l.Parent)
                .WithMany(l => l.SubLocations).HasForeignKey(sl => sl.ParentId)
                .WillCascadeOnDelete(false);

            this.HasMany(l => l.Buildings)
                .WithRequired(b => b.Location).HasForeignKey(b => b.LocationId)
                .WillCascadeOnDelete(false);
        }
    }
}
