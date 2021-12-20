using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Daikin.SSO.Portable
{
    public class AuthorizationRoleRequirement : IAuthorizationRequirement
    {
        #region Properties
        public List<string> Roles { get; }
        #endregion

        #region Constructors
        // This can handle the case where a list of roles comes in with on string delimited, or an array of strings, or combination
        public AuthorizationRoleRequirement(params string[] roles)
        {

            Roles = new List<string>();

            foreach (var role in roles)
            {
                var roleChecks = role.Split(AuthorizationPolicyProvider.PolicyDelimiter);
                foreach (var roleCheck in roleChecks)
                {
                    Roles.Add(roleCheck);
                }
            }
        }
        #endregion
    }
}
