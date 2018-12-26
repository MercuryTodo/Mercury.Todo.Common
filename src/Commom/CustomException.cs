using System;

namespace Common
{
    public abstract class CustomException : Exception
    {
        public string Code { get; }
xxxxxxxxxx
        protected CustomException()
        {
        }

        protected CustomException(string code)
        {
            Code = code;
        }

        protected CustomException(string message, params object[] args) : this(string.Empty, message, args)
        {
        }

        protected CustomException(string code, string message, params object[] args) : this(null, code, message, args)
        {
        }

        protected CustomException(Exception innerException, string message, params object[] args)
            : this(innerException, string.Empty, message, args)
        {
        }

        protected CustomException(Exception innerException, string code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            Code = code;
        }
    }
}
