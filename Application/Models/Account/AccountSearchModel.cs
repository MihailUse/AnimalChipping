using Application.Models.Common;

namespace Application.Models.Account;

public class AccountSearchModel : ListModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}