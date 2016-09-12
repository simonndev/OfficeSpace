using OfficeSpace.DataAccess.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace OfficeSpace.DataAccess.Configurations
{
    public class WorkspaceTypeConfiguration : EntityTypeConfiguration<WorkspaceType>
    {
        public WorkspaceTypeConfiguration()
        {
            this.HasKey(wst => wst.Id);
            this.Property(wst => wst.Id)
                .HasColumnName("TypeId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(wst => wst.Name)
                .HasColumnName("TypeName")
                .IsRequired()
                .HasMaxLength(255);

            this.HasMany(wst => wst.Workspaces)
                .WithRequired(ws => ws.WorkspaceType).HasForeignKey(ws => ws.WorkspaceTypeId)
                .WillCascadeOnDelete(false);
        }
    }
}
