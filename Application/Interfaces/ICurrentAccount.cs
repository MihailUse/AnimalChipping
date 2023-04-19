using Domain.Entities;

namespace Application.Interfaces;

public interface ICurrentAccount
{
    Account? Account { get; set; }
}