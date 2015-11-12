using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Accuracy.Core.Models.Mapping
{
    public class UserImageMap : EntityTypeConfiguration<UserImage>
    {
        public UserImageMap()
        {
            // Primary Key

            // Table & Column Mappings
            this.ToTable("UserImages");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ImageId).HasColumnName("ImageId");
            this.Property(t => t.Username).HasColumnName("Username");
        }
    }
}
