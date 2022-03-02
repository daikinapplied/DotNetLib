using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Daikin.DotNetLib.Security
{
    public class RoleJsonClaimTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var claimsIdentity = new ClaimsIdentity();
            var roleClaims = principal.Claims.Where(claim => claim.Type == RoleJsonAuthorizationPolicyProvider.PolicyClaimType);
            foreach (var roleClaim in roleClaims)
            {
                var roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(roleClaim.Value);
                if (roles == null) continue;
                foreach (var role in roles)
                {
                    if (!principal.HasClaim(claim => claim.Type == RoleJsonAuthorizationPolicyProvider.PolicyClaimType && claim.Value == role))
                    {
                        claimsIdentity.AddClaim(new Claim(RoleJsonAuthorizationPolicyProvider.PolicyClaimType, role));
                    }
                }
            }
            principal.AddIdentity(claimsIdentity);
            return Task.FromResult(principal);
        }
    }
}
