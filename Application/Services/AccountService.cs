using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Account;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AccountService : IAccountService
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

    public async Task<AccountModel> CreateAccount(CreateAccountModel createModel)
    {
        var account = _mapper.Map<Account>(createModel);

        var isExistsEmail = await _context.Accounts.AnyAsync(x => x.Email == account.Email);
        if (isExistsEmail)
            throw new ConflictException();

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        return _mapper.Map<AccountModel>(account);
    }

    public async Task<AccountModel> GetAccount(int accountId)
    {
        var account = await _context.Accounts
            .ProjectTo<AccountModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == accountId);

        if (account == default)
            throw new NotFoundException();

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

    public async Task<AccountModel> Update(int id, AccountUpdateModel updateModel)
    {
        if (_currentAccount.Account?.Id == default)
            throw new AccessDenied();

        var account = _mapper.Map<Account>(updateModel);
        account.Id = id;

        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();

        return _mapper.Map<AccountModel>(account);
    }

    public async Task<Account> Authenticate(string email, string password)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        if (account == default)
            throw new NotFoundException();

        return account;
    }
}