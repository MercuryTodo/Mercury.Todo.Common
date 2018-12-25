using System;

namespace Common.Consumers
{
    public interface IExceptionHandler
    {
        void Handle(Exception exception, params string[] tags);

        void Handle(Exception exception, object data,
            string name = "Request details", params string[] tags);
    }
}