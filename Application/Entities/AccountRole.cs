// ReSharper disable InconsistentNaming

namespace Application.Entities;

[Flags]
public enum AccountRole
{
    ADMIN = 1,
    CHIPPER = 2,
    USER = 4
}