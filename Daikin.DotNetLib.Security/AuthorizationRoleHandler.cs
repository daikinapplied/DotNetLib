using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Daikin.SSO.Portable
{
    public class AuthorizationRoleHandler : AuthorizationHandler<AuthorizationRoleRequirement>
    {
        #region Constants
        public const string PolicyName = "RolePolicy";
        #endregion

        #region Fields
        private readonly ILogger<AuthorizationRoleHandler> _logger;
        #endregion

        #region Constructors
        public AuthorizationRoleHandler(ILogger<AuthorizationRoleHandler> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Methods
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRoleRequirement requirement)
        {
            if (HasAccess(context.User, requirement.Roles.ToArray()))
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation($"No access via {AuthorizationPolicyProvider.PolicyClaim} claim check");
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Function
        public static bool HasAccess(ClaimsPrincipal user, params string[] rolesToCheck)
        {
            var rolesAssigned = Get(user);
            return rolesToCheck.Any(roleToCheck => rolesAssigned.Contains(roleToCheck));
        }

        public static List<string> Get(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(c => c.Type == AuthorizationPolicyProvider.PolicyClaim);
            return claim == null ? new List<string>() : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(claim.Value);
        }
        #endregion
    }
}
