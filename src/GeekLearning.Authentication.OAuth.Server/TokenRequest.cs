using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace GeekLearning.Authentication.OAuth.Server
{
    public class TokenRequest
    {
        public string Client_Id { get;  set; }
        public string Client_Secret { get;  set; }
        public string Code { get;  set; }
        public string Refresh_Token { get;  set; }
        public string Grant_Type { get;  set; }
        public string Redirect_Uri { get;  set; }
    }
}
