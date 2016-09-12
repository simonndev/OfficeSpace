using OfficeSpace.DataAccess.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace OfficeSpace.DataAccess.Configurations
{
    public class FloorConfiguration : EntityTypeConfiguration<Floor>
    {
        public FloorConfiguration()
        {
            this.HasKey(f => f.Id);
            this.Property(f => f.Id)
                .HasColumnName("FloorId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(f => f.Name)
                .HasColumnName("FloorName")
                .IsRequired()
                .HasMaxLength(255);

            this.Property(f => f.Code)
                .HasColumnName("FloorCode")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("Idx_Floor_Code", 1)
                        { IsUnique = true }));

            this.HasMany(f => f.Units)
                .WithRequired(u => u.Floor).HasForeignKey(u => u.FloorId)
                .WillCascadeOnDelete(false);
        }
    }
}
