using Application.Models.Common;

namespace Application.Models.Animal;

public class AnimalSearchModel : ListModel
{
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public int? ChipperId { get; set; }
    public long? ChippingLocationId { get; set; }
    public string? Gender { get; set; }
    public string? LifeStatus { get; set; }
}