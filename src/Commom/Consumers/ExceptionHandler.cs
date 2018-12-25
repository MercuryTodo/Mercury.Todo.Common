using System;

namespace Common.Consumers
{
    public class ExceptionHandler : IExceptionHandler
    {
        public void Handle(Exception exception, params string[] tags)
        {
            throw exception;
        }

        public void Handle(Exception exception, object data, string name = "Request details", params string[] tags)
        {
            throw exception;
        }
    }
}