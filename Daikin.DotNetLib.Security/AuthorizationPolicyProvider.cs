using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Daikin.DotNetLib.Security
{
    public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        #region Constants
        public const string PolicyAttributePrefix = "Role";
        public const char PolicyDelimiter = ',';
        public const string PolicyClaim = "role";
        #endregion

        #region Properties
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        #endregion

        #region Constructors
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // ASP.NET Core only uses one authorization policy provider, so if the custom implementation doesn't handle all
            // policies (including default policies, etc.) it should fall back to an alternate provider.  In this case, a
            // default authorization policy provider (constructed with options from the dependency injection container) is
            // used if this custom provider isn't able to handle a given policy name.  If a custom policy provider is able
            // to handle all expected policy names then, of course, this fallback pattern is unnecessary.
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        #endregion

        #region Methods
        // Policies are looked up by string name, so expect 'parameters' (like roles) to be embedded in the policy names.
        // This is abstracted away from developers by the more strongly-typed attributes derived from AuthorizeAttribute
        // (like [RolePolicyAuthorize] in this sample)
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // If the policy name doesn't match the format expected by this policy provider, try the fallback provider.
            // If no fallback provider is used, this would return Task.FromResult<AuthorizationPolicy>(null) instead.
            if (!policyName.StartsWith(PolicyAttributePrefix, StringComparison.OrdinalIgnoreCase)) return FallbackPolicyProvider.GetPolicyAsync(policyName);

            var roleNameString = policyName.Substring(PolicyAttributePrefix.Length);
            if (roleNameString.Length == 0) return Task.FromResult<AuthorizationPolicy>(null);

            var policy = new AuthorizationPolicyBuilder();
            var roleNames = roleNameString.Split(PolicyDelimiter);
            foreach (var roleName in roleNames)
            {
                policy.AddRequirements(new AuthorizationRoleRequirement(roleName));
            }
            return Task.FromResult(policy.Build());
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();
        #endregion
    }
}
