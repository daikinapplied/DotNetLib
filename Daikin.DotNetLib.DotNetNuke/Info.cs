using System;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace Daikin.DotNetLib.DotNetNuke
{
    public static class Info
    {
        public static string GetCurrentUserName()
        {
            try
            {
                return UserController.Instance.GetCurrentUserInfo().Username.ToLower();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static int GetCurrentUserId()
        {
            try
            {
                return UserController.Instance.GetCurrentUserInfo().UserID;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int GetCurrentPortalId()
        {
            try
            {
                return PortalController.Instance.GetCurrentPortalSettings().PortalId;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
