using Application.Interfaces;
using Domain.Entities;

namespace WebApi.Services;

public class CurrentAccountService : ICurrentAccount
{
    public Account? Account { get; set; }
}