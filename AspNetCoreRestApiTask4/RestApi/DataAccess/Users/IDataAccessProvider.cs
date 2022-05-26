namespace RestApi.DataAccess.Users;

using RestApi.Models.Users;

public interface IUserDataAccessProvider
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(int id);
    void Register(RegisterRequest model);
    void Update(int id, UpdateRequest model);
    void Delete(int id);
}
