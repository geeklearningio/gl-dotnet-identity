namespace GeekLearning.Authentication.OAuth.Server.Mvc
{
    using Microsoft.AspNetCore.Mvc;

    public static class OAuthServerControllerExtensions
    {
        public static AuthorizationCodeResult AuthorizationCode(this Controller controller, AuthorizationRequest request)
        {
            return new AuthorizationCodeResult(request);
        }
    }
}
