using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPlus.Source.Enums
{
    public enum ClientResponse
    {
        INVALIDPASSWORD = 0,
        INVALIDUSERNAME = 1,
        AUTHENABLED = 2,
        SUCCESSFUL = 3,
        UNKNOWNERROR = 4,
        RETRY = 5,
    }
}
