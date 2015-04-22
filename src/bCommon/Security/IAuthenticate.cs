using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bCommon.Security
{
    public interface IAuthenticate
    {
        Boolean ChangePassword(string userId, string email, string oldPassword, string newPassword);
        Boolean CreateAccount(string userId, string password, string email);
        Boolean DisableAccount(string userId, string email);
        object GetAccount(string userId, string email);
        String ResetPassword(string userId, string email);
        Boolean Authenticate(string userId, string email, string password);
    }
}
