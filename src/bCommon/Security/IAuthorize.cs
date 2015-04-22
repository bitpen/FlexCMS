using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bCommon.Security
{
    public interface IAuthorize
    {
        List<object> GetAccountRoles(object accountDetails);
        Boolean IsAccountInRole(object accountDetails);
        List<object> GetAccountsInRole(object roleDetails);
        List<object> GetAccoutnsInRoles(List<object> roleDetails);
        List<object> GetAllRoles();
    }
}
