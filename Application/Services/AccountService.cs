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

    public AccountService(IMapper mapper, IDatabaseContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<AccountModel> CreateAccount(CreateAccountModel createAccountModel)
    {
        var account = _mapper.Map<Account>(createAccountModel);

        var isExistsEmail = await _context.Accounts.AnyAsync(x => x.Email == account.Email);
        if (isExistsEmail)
            throw new AlreadyExistsException();

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        return _mapper.Map<AccountModel>(account);
    }

    public async Task<AccountModel> GetAccount(Guid accountId)
    {
        var account = await _context.Accounts
            .ProjectTo<AccountModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (account == default)
            throw new NotFoundException();

        return account;
    }
}