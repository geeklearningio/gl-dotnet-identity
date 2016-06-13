using GeekLearning.Authentication.OAuth.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace GeekLearning.Identity.Sample.Services
{
    public class ClientValidationResult : IClientValidationResult
    {
        public ClaimsIdentity Identity { get; set; }

        public bool Success { get; set; }
    }
}
