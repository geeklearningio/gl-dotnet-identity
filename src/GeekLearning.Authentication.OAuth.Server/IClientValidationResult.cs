using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GeekLearning.Authentication.OAuth.Server
{
    public interface IClientValidationResult
    {
        bool Success { get; set; }

        ClaimsIdentity Identity { get; set; }
    }
}
