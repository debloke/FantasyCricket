using FantasyCricket.Models;

namespace FantasyCricket.Service
{
    public interface IUser
    {
        void CreateUser(string username,string password, string displayName);
        void LoginUser(string username,string password);
}
}
