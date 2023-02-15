using System.ComponentModel.DataAnnotations;

namespace Application.Models.Account;

public class AccountSearchModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public int From { get; set; } = 0;
    public int Size { get; set; } = 10;
}