namespace GeekLearning.Authentication.OAuth.Server
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IClientProvider
    {
        Task<string[]> GetClientAudiences(string client_Id);

        Task<IClientValidationResult> ValidateCredentialsAsync(string nameIdentifier, string client_Id, string client_Secret);

        Task OnTokenEmitted(string client_Id, string nameIdentifier, IEnumerable<IToken> tokens);

        Task<IClientValidationResult> ValidateTokenAsync(string tokenId, string nameIdentifier);
    }
}
