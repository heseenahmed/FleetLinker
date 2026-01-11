using FleetLinker.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetLinker.Domain.Entity
{
    public class EquipmentSparePart : BaseEntity
    {
        [Required]
        public PartType Type { get; set; } // نوع القطعة (أصلي أم تجاري)

        [Required]
        public string PartNumber { get; set; } = null!; // رقم القطعة (إلزامي)

        public string? BrandAr { get; set; } // البراند بالعربية

        public string? BrandEn { get; set; } // Brand in English

        public int YearOfManufacture { get; set; } // سنة الصنع

        public string? AssetNumber { get; set; } // رقم الأصل

        [Required]
        public string SupplierId { get; set; } = null!; // Supplier User ID

        [ForeignKey("SupplierId")]
        public virtual ApplicationUser Supplier { get; set; } = null!;
    }
}
