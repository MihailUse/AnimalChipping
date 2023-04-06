using Application.Entities;
using Application.Interfaces;

namespace WebApi.Services;

public class CurrentAccountService : ICurrentAccount
{
    public Account? Account { get; set; }
}