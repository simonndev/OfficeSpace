using OfficeSpace.DataAccess.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace OfficeSpace.DataAccess.Configurations
{
    public class UnitConfiguration : EntityTypeConfiguration<Unit>
    {
        public UnitConfiguration()
        {
            this.HasKey(u => u.Id);
            this.Property(u => u.Id)
                .HasColumnName("UnitId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(u => u.Name)
                .HasColumnName("UnitName")
                .IsRequired()
                .HasMaxLength(255);

            this.Property(u => u.Code)
                .HasColumnName("UnitCode")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("Idx_Unit_Code", 1)
                        { IsUnique = true }));

            this.HasMany(u => u.Workspaces)
                .WithRequired(ws => ws.Unit).HasForeignKey(ws => ws.UnitId)
                .WillCascadeOnDelete(false);
        }
    }
}
