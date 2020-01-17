using System;

namespace RecursiveGeek.DotNetLib.Network
{
    /// <summary>
    /// A custom exception for Emails that is thrown
    /// if an Email is not formatted properly.
    /// This is usually determined using the static method Email.ValidAddress(string EmailAddr)
    /// </summary>
    [Serializable]
    public class EmailFormatException : Exception
    {
        #region Constructors
        public EmailFormatException() : base()
        {
        }

        public EmailFormatException(string message) : base(message)
        {
        }

        protected EmailFormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}
