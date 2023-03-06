using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Account;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

internal class AccountService : IAccountService
{
    private readonly IMapper _mapper;
    private readonly IDatabaseContext _context;
    private readonly IDatabaseFunctions _databaseFunctions;
    private readonly ICurrentAccount _currentAccount;

    public AccountService(
        IMapper mapper,
        IDatabaseContext context,
        IDatabaseFunctions databaseFunctions,
        ICurrentAccount currentAccount
    )
    {
        _mapper = mapper;
        _context = context;
        _databaseFunctions = databaseFunctions;
        _currentAccount = currentAccount;
    }

    public async Task<AccountModel> Get(int accountId)
    {
        var account = await _context.Accounts
            .ProjectTo<AccountModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == accountId);

        if (account == default)
            throw new NotFoundException("Account not found");

        return account;
    }

    public async Task<List<AccountModel>> Search(AccountSearchModel searchModel)
    {
        IQueryable<Account> query = _context.Accounts;

        if (searchModel.FirstName != default)
            query = query.Where(x => _databaseFunctions.ILike(x.FirstName, $"%{searchModel.FirstName}%"));

        if (searchModel.LastName != default)
            query = query.Where(x => _databaseFunctions.ILike(x.LastName, $"%{searchModel.LastName}%"));

        if (searchModel.Email != default)
            query = query.Where(x => _databaseFunctions.ILike(x.Email, $"%{searchModel.Email}%"));

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

        var emailExists = await _context.Accounts.AnyAsync(x => x.Email == account.Email);
        if (emailExists)
            throw new ConflictException("Email already exists");

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        return _mapper.Map<AccountModel>(account);
    }

    public async Task<AccountModel> Update(int accountId, AccountUpdateModel updateModel)
    {
        if (_currentAccount.Account?.Id == default || _currentAccount.Account!.Id != accountId)
            throw new AccessDenied("Permission denied");

        var account = _mapper.Map<Account>(updateModel);
        account.Id = accountId;

        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();

        return _mapper.Map<AccountModel>(account);
    }

    public async Task<Account> Authenticate(string email, string password)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        if (account == default)
            throw new NotFoundException("Account not found");

        return account;
    }

    public async Task Delete(int accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == default)
            throw new NotFoundException("Account not found");

        if (_currentAccount.Account?.Id == default || _currentAccount.Account!.Id != accountId)
            throw new AccessDenied("Permission denied");

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckExists(string email, string password)
    {
        return await _context.Accounts.AnyAsync(x => x.Email == email && x.Password == password);
    }
}