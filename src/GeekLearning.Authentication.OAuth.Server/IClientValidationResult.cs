namespace GeekLearning.Authentication.OAuth.Server
{
    using System.Security.Claims;

    public interface IClientValidationResult
    {
        bool Success { get; set; }

        ClaimsIdentity Identity { get; set; }
    }
}
