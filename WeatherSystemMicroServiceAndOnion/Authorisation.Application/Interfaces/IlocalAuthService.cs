namespace Authorisation.Application.Interfaces
{
    public interface ILocalAuthServices
    {
        public bool ValidateLocalUser(string username, string password);
        public bool RegisterLocalUser(string username, string password);
        void RegisterExternalUser(string v);
    }
}
