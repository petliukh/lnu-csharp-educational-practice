namespace RestApi.DataAccess.BlackListedTokens;

using RestApi.Models.BlackListedTokens;

public class BlackListedTokenService : IBlackListedTokenService
{
    private readonly BlackListedTokenPostgreSqlContext _context;

    public BlackListedTokenService(BlackListedTokenPostgreSqlContext context)
    {
        _context = context;
    }

    public void InvalidateToken(BlackListedToken blackListedToken)
    {
        _context.BlackListedTokens.Add(blackListedToken);
        _context.SaveChanges();
    }

    public List<BlackListedToken> GetBlackListedTokens()
    {
        return _context.BlackListedTokens.ToList();
    }
}