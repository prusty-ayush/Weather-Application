namespace Authorisation.Domain.Interfaces
{
    public interface IUserRepository
    {
        public bool UserExist(string username);
        public void AddUser(string username, string passwordhash, bool isExternalUser = false);
        public string? GetPasswordHash(string username);
    }
}
