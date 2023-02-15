namespace Application.Interfaces;

public interface IDatabaseFunctions
{
    bool ILike(string matchExpression, string pattern);
}