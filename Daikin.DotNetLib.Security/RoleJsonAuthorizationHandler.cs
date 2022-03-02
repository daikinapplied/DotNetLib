using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Daikin.DotNetLib.Security
{
    public class RoleJsonAuthorizationHandler : AuthorizationHandler<RoleJsonAuthorizationRequirement>
    {
        #region Fields
        private readonly ILogger<RoleJsonAuthorizationHandler> _logger;
        #endregion

        #region Constructors
        public RoleJsonAuthorizationHandler(ILogger<RoleJsonAuthorizationHandler> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Methods
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleJsonAuthorizationRequirement requirement)
        {
            if (HasAccess(context.User, requirement.Roles.ToArray()))
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation($"No access via {RoleJsonAuthorizationPolicyProvider.PolicyClaimType} claim check");
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
            var claim = user.FindFirst(c => c.Type == RoleJsonAuthorizationPolicyProvider.PolicyClaimType);
            return claim == null ? new List<string>() : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(claim.Value);
        }
        #endregion
    }
}
