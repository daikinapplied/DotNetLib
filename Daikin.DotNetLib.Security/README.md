# Introduction
<img src="Images/security.png" width="64" align="right" alt="Security Project Logo"/>
This is a Security library that is written in C# using .NET Standard.  Its purpose is to make a little easier to encrypt and decrypt data (symmetrical, asymmtrical, and machine-based).

# Purpose
Provide helper tools with OAuth2 and Encryption/Decryption.  This works with .NET 6.0 (currently based on the .NET 5.0 template).

## RoleJson Helpers
This assists with using the **role** claim type have a claim value of a JSON array.  

  ```
  role = ["rolename1", "rolename2", ...]
  ```

This is useful in minimizing the size of an OAuth2 token when there are a large number of roles.  However, this doesn't work as well with later versions of .NET Core, which run with each role be specified in the following format:

  ```
  role = "rolename1"
  role = "rolename2"
  ```

### RoleJsonClaimTransformation
This assists with transforming role as a JSON array to a role following the format, with the original JSON array remaining:
  ```
  role = ["rolename1", "rolename2", ...] // Original JSON Array
  role = "rolename1" // Transformed
  role = "rolename2" // Transformed
  ...
  ```

To implement, the following code must be inserted:
  ```
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddTransient<IClaimsTransformation, RoleJsonClaimTransformation>();
    services
      .AddAuthentication(options =>
        {
           options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
           options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
      .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
           options.AccessDeniedPath = "/Error";
        })
      .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
          options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.Authority = "https://sso.myserver.local";
          options.ClientId = "clientid";
          options.ClientSecret = "clientsecret";
          options.ResponseType = "code id_token"; // Hybrid Grant Type
          options.Scope.Add("openid email profile offline_access role");
          options.ClaimActions.MapJsonKey(RoleJsonAuthorizationPolicyProvider.PolicyClaimType, RoleJsonAuthorizationPolicyProvider.PolicyClaimType, RoleJsonAuthorizationPolicyProvider.PolicyClaimType)
          options.TokenValidationParameters.RoleClaimType = RoleJsonAuthorizationPolicyProvider.PolicyClaimType;
          options.TokenValidationParameters.NameClaimType = "name";
          options.RequireHttpsMetadata = true;
          options.SaveTokens = true;
          options.GetClaimsFromUserInfoEndpoint = true;
        });
      services.AddAuthorization(options =>
        {
           var rolePolicy = new AuthorizationPolicyBuilder()
               .RequireAuthenticatedUser()
               .RequireClaim(Security.RoleJsonAuthorizationPolicyProvider.PolicyClaimType, RoleClaimValueViewer, RoleClaimValueApprover)
               .Build();

           options.AddPolicy(RoleJsonAuthorizationPolicyProvider.PolicyName, rolePolicy);
           options.DefaultPolicy = rolePolicy; // if not policy specified, use this policy
        });

      // If using Razor Pages
      services.AddRazorPages()
        .AddRazorPagesOptions(options => // Add Page/Folder Security
          {
            options.Conventions.AuthorizeFolder("/", RoleJsonAuthorizationPolicyProvider.PolicyName); // locks down the entire site
            options.Conventions.AllowAnonymousToPage("/Error");
          });
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    app.UseCookiePolicy(new CookiePolicyOptions { Secure = CookieSecurePolicy.Always });

    app.UseRouting();
    app.UseAuthentication(); // Added before app.UseAuthorization() to include /signin-oidc endpoint for callback from OpenIdConnect/OAuth2
    app.UseAuthorization();
  }
  ```

Of particular interest is the *RoleJsonAuthorizaitonHandler.HasAccess()* also works, because of the remaining of the JSON array, for testing.

### RoleJsonAuthorization*
Assists with role-based authorization from a JSON array, but is more complicated than **RoleJsonClaimTransformation*, but has benefits for both MVC and Razor approaches.

  ```
  public void ConfigureServices(IServiceCollection services)
  {
    // Replace the default authorization policy provider with our own custom provider which can return authorization policies for given policy names (instead of using the default policy provider)
    // This is used for policy attribute support, such as [RoleAuthorize]
    services.AddSingleton<IAuthorizationPolicyProvider, RoleJsonAuthorizationPolicyProvider>();

    // Handlers must be provided for the requirements of the authorization policies
    services.AddSingleton<IAuthorizationHandler, RoleJsonAuthorizationHandler>();

    services
      .AddAuthentication(options =>
        {
          options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
      .AddCookie(options =>
        {
          options.AccessDeniedPath = "/Error";
        })
      .AddOpenIdConnect(options =>
        {
          options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.Authority = "https://sso.myserver.local";
          options.ClientId = "clientid";
          options.ClientSecret = "clientsecret";
          options.ResponseType = "code id_token"; // Hybrid Grant Type
          options.Scope.Add("openid email profile offline_access role");
          options.ClaimActions.MapJsonKey(RoleJsonAuthorizationPolicyProvider.PolicyClaimType, RoleJsonAuthorizationPolicyProvider.PolicyClaimType, RoleJsonAuthorizationPolicyProvider.PolicyClaimType)
          options.RequireHttpsMetadata = true;
          options.SaveTokens = true;
          options.GetClaimsFromUserInfoEndpoint = true;
        });
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    app.UseRouting();
    app.UseAuthentication(); // Added before app.UseAuthorization() to include /signin-oidc endpoint for callback from OpenIdConnect/OAuth2
    app.UseAuthorization();
  }
  ```

# Documentation Overview
Check out the primary [README.md](../README.md) documentation.

~ end ~
