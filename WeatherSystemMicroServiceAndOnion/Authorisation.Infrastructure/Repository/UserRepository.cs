using Authorisation.Domain.Interfaces;
using Authorisation.Infrastructure.Data;

namespace Authorisation.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        //private static Dictionary<string, string> users = new Dictionary<string, string>();

        //public bool UserExist(string username)
        //{
        //    return users.ContainsKey(username);
        //}

        //public void AddUser(string username, string passwordhash)
        //{
        //    users.Add(username, passwordhash);

        //}
        //public string? GetPasswordHash(string username)
        //{
        //    users.TryGetValue(username, out string? password);
        //    return password;
        //}

        private readonly AppDbContext db;

        public UserRepository(AppDbContext _db) 
        {
            db = _db;
        }

        public bool UserExist(string username)
        {
            return db.users.Any(u => u.Username == username);
        }

        public void AddUser(string username, string passwordhash, bool isExternalUser = false)
        {
            db.users.Add(new Domain.Entities.User 
            { 
                Username = username, 
                PasswordHash = passwordhash, 
                IsExternalUser = isExternalUser 
            });
            db.SaveChanges();
        }

        public string? GetPasswordHash(string username)
        {
            return db.users.FirstOrDefault(s=>s.Username==username)?.PasswordHash;
        }
    }
}
