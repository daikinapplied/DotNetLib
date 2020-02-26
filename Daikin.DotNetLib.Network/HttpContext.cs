using System;

namespace Daikin.DotNetLib.Network
{
    public static class HttpContext
    {
        #region Fields
        private static Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Properties

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                if (_httpContextAccessor == null)
                {
                    throw new Exception("Must call Daikin.DotNetLib.Network.HttpContext.Configure() pror to accessing Daikin.DotNetLib.Network.HttpContext.Current");
                }
                return _httpContextAccessor.HttpContext;
            }
        }

        #endregion

        #region Funtions
        public static void Configure(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion
    }
}
