using System;

namespace Common.Consumers
{
    public class HandlerException : CustomException
    {
        public HandlerException()
        {
        }

        public HandlerException(string code) : base(code)
        {
        }

        public HandlerException(string message, params object[] args) : base(string.Empty, message, args)
        {
        }

        public HandlerException(string code, string message, params object[] args) : base(code, message, args)
        {
        }

        public HandlerException(Exception innerException, string message, params object[] args)
            : base(innerException, string.Empty, message, args)
        {
        }

        public HandlerException(Exception innerException, string code, string message, params object[] args)
            : base(innerException, code, string.Format(message, args), args)
        {
        }
    }
}