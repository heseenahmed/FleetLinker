using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetLinker.Domain.Entity
{
    public class Equipment : BaseEntity
    {
        [Required]
        public string Brand { get; set; } = null!; // البراند (العلامة التجارية)
        
        [Required]
        public int YearOfManufacture { get; set; } // سنة الصنع
        
        [Required]
        public string ChassisNumber { get; set; } = null!; // رقم الشاسيه
        
        [Required]
        public string Model { get; set; } = null!; // الطراز
        
        public string? AssetNumber { get; set; } // رقم الأصل (إن وجد)

        [Required]
        public string OwnerId { get; set; } = null!; // Equipment Owner User ID

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; } = null!;
    }
}
