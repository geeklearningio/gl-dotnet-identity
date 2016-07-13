namespace GeekLearning.Authentication.OAuth.Server
{
    public interface ITokenValidationResult
    {
        bool Success { get; set; }

        string NameIdentifier { get; set; }

        string Usage { get; set; }

        string Id { get; set; }
    }
}
