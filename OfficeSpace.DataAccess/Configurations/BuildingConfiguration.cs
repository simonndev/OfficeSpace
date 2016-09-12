using OfficeSpace.DataAccess.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace OfficeSpace.DataAccess.Configurations
{
    public class BuildingConfiguration : EntityTypeConfiguration<Building>
    {
        public BuildingConfiguration()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Id)
                .HasColumnName("BuildingId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(b => b.Name)
                .HasColumnName("BuildingName")
                .IsRequired()
                .HasMaxLength(255);

            this.Property(b => b.Code)
                .HasColumnName("BuildingCode")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("Idx_Building_Code", 1)
                        { IsUnique = true }));

            this.HasMany(b => b.Floors)
                .WithRequired(f => f.Building).HasForeignKey(f => f.BuildingId)
                .WillCascadeOnDelete(false);
        }
    }
}
