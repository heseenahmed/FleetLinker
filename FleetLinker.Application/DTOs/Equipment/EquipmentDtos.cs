namespace FleetLinker.Application.DTOs.Equipment
{
    public class EquipmentDto
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = null!;
        public int YearOfManufacture { get; set; }
        public string ChassisNumber { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? AssetNumber { get; set; }
        public string OwnerId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
    }

    public class CreateEquipmentDto
    {
        public string Brand { get; set; } = null!;
        public int YearOfManufacture { get; set; }
        public string ChassisNumber { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? AssetNumber { get; set; }
    }

    public class UpdateEquipmentDto
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = null!;
        public int YearOfManufacture { get; set; }
        public string ChassisNumber { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? AssetNumber { get; set; }
    }
}
