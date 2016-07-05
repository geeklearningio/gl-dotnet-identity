namespace GeekLearning.Authentication.OAuth.Server
{
    using System;

    public interface IToken
    {
        string Id { get; }

        string Token { get; }

        DateTimeOffset FromDate { get; }

        DateTimeOffset ToDate { get; }

        string Type { get; }
    }
}
