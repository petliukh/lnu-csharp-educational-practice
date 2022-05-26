namespace RestApi.DataAccess.BlackListedTokens;

using RestApi.Models.BlackListedTokens;

public interface IBlackListedTokenService
{
    List<BlackListedToken> GetBlackListedTokens();
    void InvalidateToken(BlackListedToken token);
}