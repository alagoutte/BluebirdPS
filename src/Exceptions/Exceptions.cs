using System;

namespace BluebirdPS.Exceptions
{
    public class BluebirdPSNullCredentialsException : Exception
    {
        public BluebirdPSNullCredentialsException(string message) : base(message)
        {
        }
    }

    public class BluebirdPSAuthenticationException : Exception
    {
        public BluebirdPSAuthenticationException(string message) : base(message)
        {
        }
    }

    public class BluebirdPSInvalidOperationException : Exception
    {
        public BluebirdPSInvalidOperationException(string message) : base(message)
        {

        }
    }

    public class BluebirdPSInvalidArgumentException : Exception
    {
        public BluebirdPSInvalidArgumentException(string message) : base(message)
        {

        }
    }
    public class BluebirdPSLimitsExceededException : Exception
    {
        public BluebirdPSLimitsExceededException(string message) : base(message)
        {

        }
    }
    public class BluebirdPSResourceViolationException : Exception
    {
        public BluebirdPSResourceViolationException(string message) : base(message)
        {

        }
    }
    public class BluebirdPSResourceNotFoundException : Exception
    {
        public BluebirdPSResourceNotFoundException(string message) : base(message)
        {

        }
    }
    public class BluebirdPSSecurityException : Exception
    {
        public BluebirdPSSecurityException(string message) : base(message)
        {

        }
    }
    public class BluebirdPSConnectionException : Exception
    {
        public BluebirdPSConnectionException(string message) : base(message)
        {

        }
    }

    public class BluebirdPSUnspecifiedException : Exception
    {
        public BluebirdPSUnspecifiedException(string message) : base(message)
        {

        }
    }
}
