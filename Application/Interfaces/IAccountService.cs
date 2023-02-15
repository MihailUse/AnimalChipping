using Application.Models.Account;
using Domain.Entities;

namespace Application.Interfaces;

public interface IAccountService
{
    Task<AccountModel> CreateAccount(CreateAccountModel createModel);
    Task<AccountModel> GetAccount(int accountId);
    Task<List<AccountModel>> Search(AccountSearchModel searchModel);
    Task<AccountModel> Update(int accountId, AccountUpdateModel updateModel);
    Task<Account> Authenticate(string email, string password);
}