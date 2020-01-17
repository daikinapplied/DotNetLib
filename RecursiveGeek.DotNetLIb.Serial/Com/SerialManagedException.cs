using System;

namespace RecursiveGeek.DotNetLib.Serial.Com
{
    public class SerialManagedException : Exception
    {
        public SerialManagedException()
        {
        }

        public SerialManagedException(string message) : base(message)
        {
        }

        public SerialManagedException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
