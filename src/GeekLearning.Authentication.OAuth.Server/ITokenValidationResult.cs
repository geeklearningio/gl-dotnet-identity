using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Authentication.OAuth.Server
{
    public interface ITokenValidationResult
    {
        bool Success { get; set; }

        string NameIdentifier { get; set; }
    }
}
