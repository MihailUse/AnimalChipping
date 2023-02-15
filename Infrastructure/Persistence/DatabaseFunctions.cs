using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DatabaseFunctions : IDatabaseFunctions
{
    public bool ILike(string matchExpression, string pattern) => EF.Functions.ILike(matchExpression, pattern);
}