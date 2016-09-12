using OfficeSpace.DataAccess.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace OfficeSpace.DataAccess.Configurations
{
    public class WorkspaceConfiguration : EntityTypeConfiguration<Workspace>
    {
        public WorkspaceConfiguration()
        {
            this.HasKey(ws => ws.Id);
            this.Property(ws => ws.Id)
                .HasColumnName("WorkspaceId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(ws => ws.Name)
                .HasColumnName("WorkspaceName")
                .IsRequired()
                .HasMaxLength(255);

            this.Property(ws => ws.Code)
                .HasColumnName("WorkspaceCode")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("Idx_Workspace_Code", 1)
                        { IsUnique = true }));
        }
    }
}
