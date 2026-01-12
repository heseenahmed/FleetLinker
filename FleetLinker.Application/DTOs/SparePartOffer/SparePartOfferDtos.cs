using FleetLinker.Domain.Enums;

namespace FleetLinker.Application.DTOs.SparePartOffer
{
    public class SparePartOfferDto
    {
        public Guid Id { get; set; }
        public Guid SparePartId { get; set; }
        public string SparePartName { get; set; } = null!;
        public string RequesterId { get; set; } = null!;
        public string RequesterName { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public OfferStatus Status { get; set; }
        public string StatusName { get; set; } = null!;
        public decimal? FinalPrice { get; set; }
        public decimal SystemPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateSparePartOfferDto
    {
        public Guid SparePartId { get; set; }
        public string? Notes { get; set; }
    }

    public class RespondToSparePartOfferDto
    {
        public Guid OfferId { get; set; }
        public decimal FinalPrice { get; set; }
        public bool SetAsSystemPrice { get; set; }
        public string? Notes { get; set; }
    }
}
