using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Authentication.OAuth.Server
{
    public interface IToken
    {
        string Id { get; }
        string Token { get; }

        DateTimeOffset FromDate { get; }
        DateTimeOffset ToDate { get; }

        string Type { get; }
    }
}
