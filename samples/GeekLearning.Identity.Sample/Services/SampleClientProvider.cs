﻿using GeekLearning.Authentication.OAuth.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Identity.Sample.Services
{
    public class SampleClientProvider : IClientProvider
    {
        public async Task<string[]> GetClientAudiences(string client_Id)
        {
            return new string[0];
        }

        public async Task OnTokenEmitted(string client_Id, string nameIdentifier, IEnumerable<IToken> tokens)
        {
        }

        public async Task<IClientValidationResult> ValidateCredentialsAsync(string nameIdentifier, string client_Id, string client_Secret)
        {
            return new ClientValidationResult
            {
                Identity = new System.Security.Claims.ClaimsIdentity(),
                Success = true
            };
        }

        public async Task<IClientValidationResult> ValidateTokenAsync(string tokenId, string nameIdentifier)
        {
            return new ClientValidationResult
            {
                Identity = new System.Security.Claims.ClaimsIdentity(),
                Success = true
            };
        }
    }
}
