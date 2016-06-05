using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Authentication.OAuth.Server
{
    public class AuthorizationRequest
    {
        public string Response_Type { get; set; }

        public string Client_Id { get; set; }

        public string Redirect_Uri { get; set; }

        public string Scope { get; set; }

        public string State { get; set; }
    }
}
