namespace Application.Models.Account;

public class AccountCreateModel : AccountRegistrationModel
{
    public string Role { get; set; } = null!;
}