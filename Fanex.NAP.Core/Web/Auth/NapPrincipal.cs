// --------------------------------------------------------------------------
// <copyright file="NapPrincipal.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
// </history>

namespace Fanex.NAP.Core.Web.Auth
{
    using System;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Security;

    /// <summary>
    /// <see cref="http://stackoverflow.com/questions/1064271/asp-net-mvc-set-custom-iidentity-or-iprincipal"/>
    /// </summary>
    public class NapPrincipal : IPrincipal
    {
        private IIdentity _identity;
        private AuthInfo _authInfo;

        public NapPrincipal(IIdentity identity, AuthInfo authInfo)
        {
            this._identity = identity;
            this._authInfo = authInfo;
        }

        public NapPrincipal(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (HttpContext.Current.User.Identity is FormsIdentity && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                HttpCookie httpCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (httpCookie == null)
                {
                    return;
                }

                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(httpCookie.Value);
                if (ticket == null || ticket.Expired || string.IsNullOrEmpty(ticket.UserData))
                {
                    return;
                }

                this._authInfo = AuthInfo.CreateUserData(ticket.UserData);
                this._identity = new FormsIdentity(ticket);
            }
        }

        public AuthInfo AuthInfo
        {
            get { return this._authInfo; }
        }

        public IIdentity Identity
        {
            get { return this._identity; }
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}