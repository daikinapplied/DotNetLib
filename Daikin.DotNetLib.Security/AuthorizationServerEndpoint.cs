using System;
using System.Collections.Generic;

namespace Daikin.SSO.Portable
{
    // TypeSafeEnum
    public sealed class AuthorizationServerEndpoint
    {
        #region Fields
        private readonly string _name;
        private readonly int _value;
        #endregion

        #region Properties
        private static readonly Dictionary<string, AuthorizationServerEndpoint> Instance = new Dictionary<string, AuthorizationServerEndpoint>();

        // Endpoint Documentation: https://github.com/IdentityModel/IdentityModel2 (scroll down)
        public static readonly AuthorizationServerEndpoint Base = new AuthorizationServerEndpoint(0, string.Empty);
        public static readonly AuthorizationServerEndpoint AuthorizeEndpoint = new AuthorizationServerEndpoint(1, "/connect/authorize");
        public static readonly AuthorizationServerEndpoint LogoutEndpoint = new AuthorizationServerEndpoint(2, "/connect/endsession");
        public static readonly AuthorizationServerEndpoint TokenEndpoint = new AuthorizationServerEndpoint(3, "/connect/token");
        public static readonly AuthorizationServerEndpoint UserInfoEndpoint = new AuthorizationServerEndpoint(4, "/connect/userinfo");
        public static readonly AuthorizationServerEndpoint IdentityTokenValiationEndpoint = new AuthorizationServerEndpoint(5, "/connect/identitytokenvalidation");
        public static readonly AuthorizationServerEndpoint TokenRevocationEndpoint = new AuthorizationServerEndpoint(6, "/connect/revocation");
        #endregion

        #region Constructors
        private AuthorizationServerEndpoint(int value, string name)
        {
            _name = name;
            _value = value;
            Instance[name] = this;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return _name;
        }

        public int GetKey()
        {
            return _value;
        }

        public string GetValue() => ToString();

        public static explicit operator AuthorizationServerEndpoint(string s)
        {
            if (Instance.TryGetValue(s, out var result)) { return result; }
            throw new InvalidCastException();
        }
        #endregion
    }
}