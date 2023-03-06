using Application.Models.Common;

namespace Application.Models.Animal;

public class AnimalSearchLocationModel : ListModel
{
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}