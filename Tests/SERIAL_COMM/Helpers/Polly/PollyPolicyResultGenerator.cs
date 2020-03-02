using Polly;
using System;

namespace TestHelper.Polly
{
    public static class PollyPolicyResultGenerator
    {
        public static PolicyResult<T> GetSuccessfulPolicy<T>() => PolicyResult<T>.Successful(default(T), new Context(Guid.NewGuid().ToString()));

        public static PolicyResult<T> GetFailurePolicy<T>(Exception exception) => PolicyResult<T>.Failure(
            exception ?? new Exception("Unable to execute your policy successfully!"),
            ExceptionType.Unhandled,
            new Context(Guid.NewGuid().ToString()));
    }
}
