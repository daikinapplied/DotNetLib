using System;

namespace Daikin.DotNetLib.Windows
{
    /// <summary>
    /// Invalid Registry Path Exception (Path is not formatted correctly)
    /// </summary>
    [Serializable]
    public class InvalidRegistryPathException : Exception
    {
        public InvalidRegistryPathException() 
        {
        }

        public InvalidRegistryPathException(string message) : base(message)
        {
        }

        protected InvalidRegistryPathException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
