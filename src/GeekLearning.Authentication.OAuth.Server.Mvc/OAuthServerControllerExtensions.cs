using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GeekLearning.Authentication.OAuth.Server.Mvc
{
    public static class OAuthServerControllerExtensions
    {
        public static AuthorizationCodeResult AuthorizationCode(this Controller controller, AuthorizationRequest request)
        {
            return new AuthorizationCodeResult(request);
        }
    }
}
