using Application.Models.Account;
using Domain.Entities;

namespace Application.Interfaces;

public interface IAccountService
{
    Task<AccountModel> Get(int accountId);
    Task<List<AccountModel>> Search(AccountSearchModel searchModel);
    Task<AccountModel> Create(AccountCreateModel accountCreateModel);
    Task<AccountModel> Registrate(AccountRegistrationModel accountCreateModel);
    Task<AccountModel> Update(int accountId, AccountUpdateModel updateModel);
    Task<Account> Authenticate(string email, string password);
    Task Delete(int accountId);
    Task<bool> CheckExists(string email, string password);
}