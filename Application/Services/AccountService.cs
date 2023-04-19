using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Account;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

internal class AccountService : IAccountService
{
    private readonly IMapper _mapper;
    private readonly IDatabaseContext _database;
    private readonly ICurrentAccount _currentAccount;

    public AccountService(
        IMapper mapper,
        IDatabaseContext database,
        ICurrentAccount currentAccount
    )
    {
        _mapper = mapper;
        _database = database;
        _currentAccount = currentAccount;
    }

    public async Task<AccountModel> Get(int accountId)
    {
        var account = await FindAccount(accountId);
        return _mapper.Map<AccountModel>(account);
    }

    public async Task<List<AccountModel>> Search(AccountSearchModel searchModel)
    {
        IQueryable<Account> query = _database.Accounts;

        if (searchModel.FirstName != default)
            query = query.Where(x => EF.Functions.ILike(x.FirstName, $"%{searchModel.FirstName}%"));

        if (searchModel.LastName != default)
            query = query.Where(x => EF.Functions.ILike(x.LastName, $"%{searchModel.LastName}%"));

        if (searchModel.Email != default)
            query = query.Where(x => EF.Functions.ILike(x.Email, $"%{searchModel.Email}%"));

        return await query
            .OrderBy(x => x.Id)
            .Skip(searchModel.From)
            .Take(searchModel.Size)
            .ProjectTo<AccountModel>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<AccountModel> Create(AccountCreateModel accountCreateModel)
    {
        var account = _mapper.Map<Account>(accountCreateModel);

        var emailExists = await _database.Accounts.AnyAsync(x => x.Email == account.Email);
        if (emailExists)
            throw new ConflictException("Email already exists");

        await _database.Accounts.AddAsync(account);
        await _database.SaveChangesAsync();

        return _mapper.Map<AccountModel>(account);
    }
    
    public async Task<AccountModel> Registrate(AccountRegistrationModel accountCreateModel)
    {
        var account = _mapper.Map<Account>(accountCreateModel);

        var emailExists = await _database.Accounts.AnyAsync(x => x.Email == account.Email);
        if (emailExists)
            throw new ConflictException("Email already exists");

        await _database.Accounts.AddAsync(account);
        await _database.SaveChangesAsync();

        return _mapper.Map<AccountModel>(account);
    }

    public async Task<AccountModel> Update(int accountId, AccountUpdateModel updateModel)
    {
        var account = await FindAccount(accountId);
        account = _mapper.Map(updateModel, account);

        _database.Accounts.Update(account!);
        await _database.SaveChangesAsync();

        return _mapper.Map<AccountModel>(account);
    }

    public async Task<Account> Authenticate(string email, string password)
    {
        var account = await _database.Accounts.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        if (account == default)
            throw new NotFoundException("Account not found");

        return account;
    }

    public async Task Delete(int accountId)
    {
        var account = await FindAccount(accountId);

        var hasAnimals = await _database.Animals.AnyAsync(x => x.ChipperId == accountId);
        if (hasAnimals)
            throw new InvalidOperationException("Account has animal");

        _database.Accounts.Remove(account);
        await _database.SaveChangesAsync();
    }

    public async Task<bool> CheckExists(string email, string password)
    {
        return await _database.Accounts.AnyAsync(x => x.Email == email && x.Password == password);
    }

    private async Task<Account> FindAccount(int accountId)
    {
        var account = await _database.Accounts.FindAsync(accountId);

        if ((accountId != _currentAccount.Account!.Id || account == default) &&
            _currentAccount.Account.Role.HasFlag(AccountRole.USER | AccountRole.CHIPPER))
            throw new ForbiddenException("Permission denied");

        if (account == default)
            throw new NotFoundException("Account not found");

        return account;
    }
}