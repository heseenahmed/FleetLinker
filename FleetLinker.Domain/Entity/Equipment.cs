using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetLinker.Domain.Entity
{
    public class Equipment : BaseEntity
    {
        public string? BrandAr { get; set; } // العلامة التجارية بالعربية
        
        [Required]
        public string BrandEn { get; set; } = null!; // Brand in English
        
        [Required]
        public int YearOfManufacture { get; set; } // سنة الصنع
        
        [Required]
        public string ChassisNumber { get; set; } = null!; // رقم الشاسيه
        
        public string? ModelAr { get; set; } // الطراز بالعربية
        
        [Required]
        public string ModelEn { get; set; } = null!; // Model in English
        
        public string? AssetNumber { get; set; } // رقم الأصل (إن وجد)

        [Required]
        public string OwnerId { get; set; } = null!; // Equipment Owner User ID

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; } = null!;

        public bool ForSale { get; set; } // معروض للبيع

        public bool ForRent { get; set; } // معروض للإيجار

        public decimal? SalePrice { get; set; } // سعر البيع

        public decimal? RentPrice { get; set; } // سعر الإيجار

        public string? ImagePath { get; set; } // صورة المعدة

        public string? Description { get; set; } // وصف المعدة
        
        public decimal? UsageHours { get; set; } // عدد ساعات الاستخدام
        
        public decimal? FuelLiters { get; set; } // عدد لترات الجاز

        public string? MechanicalId { get; set; } // ID الفني الذي أدخل البيانات
        
        [ForeignKey("MechanicalId")]
        public virtual ApplicationUser? Mechanical { get; set; }
    }
}
