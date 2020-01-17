namespace Daikin.DotNetLib.Application
{
    public class StatusResult
    {
        #region Enumerators
        public enum Status // Used for logging and result codes to callers
        {
            Success = 0, // Used as a success response
            Failure = 2701, // General
            InfoHttpRequest = 2702
        }
        #endregion

        #region Fields
        #endregion

        #region Properties
        public int StatusCode { get; private set; }
        public string Message { get; private set; }
        #endregion

        #region Constructors
        public StatusResult()
        {
            Message = string.Empty;
            StatusCode = (int)Status.Success;
        }

        public StatusResult(string message, int statusCode)
        {
            UpdateStatusIfOkay(message, statusCode);
        }

        public StatusResult(string message, Status statusCode = Status.Success)
        {
            UpdateStatusIfOkay(message, (int)statusCode);
        }
        #endregion

        #region Methods
        public void MergeIfOkay(StatusResult statusResult)
        {
            if (!IsOkay()) return; // Update only if something not already documented
            StatusCode = statusResult.StatusCode;
            Message = statusResult.Message;
        }

        public void UpdateStatusIfOkay(string message, int statusCode)
        {
            if (!IsOkay()) return; // Update only if something not already documented
            Message = message;
            StatusCode = statusCode;
        }

        public void UpdateStatusIfOkay(string message, Status statusCode)
        {
            UpdateStatusIfOkay(message, (int)statusCode);
        }

        public void UpdateStatusIfOkay(string message)
        {
            // This is a unique situation where everything is okay but want the message (e.g. FOB 01)
            if (IsOkay() && string.IsNullOrEmpty(Message)) { Message = message; }
        }

        public bool IsOkay()
        {
            return (StatusCode == (int)Status.Success);
        }
        #endregion
    }
}
