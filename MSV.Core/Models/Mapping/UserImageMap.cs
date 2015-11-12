using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Accuracy.Core.Models.Mapping
{
    public class ImageMap : EntityTypeConfiguration<Image>
    {
        public ImageMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("Images");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ImageUrl).HasColumnName("ImageUrl");
            this.Property(t => t.External).HasColumnName("External");
            this.Property(t => t.Index).HasColumnName("Index");
        }
    }
}
