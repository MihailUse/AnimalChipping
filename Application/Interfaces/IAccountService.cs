using Application.Models.Account;

namespace Application.Interfaces;

public interface IAccountService
{
    Task<AccountModel> CreateAccount(CreateAccountModel createAccountModel);
    Task<AccountModel> GetAccount(Guid accountId);
}