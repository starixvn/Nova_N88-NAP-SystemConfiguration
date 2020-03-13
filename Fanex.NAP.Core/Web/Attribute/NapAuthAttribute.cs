// --------------------------------------------------------------------------
// <copyright file="NapAuthAttribute.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kos.Nguyen" date="2012.8.14">Created</change>
// </history>

namespace Fanex.NAP.Core.Web.Attribute
{
    using System;
    using System.Web;
    using System.Web.Security;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Auth;

    /// <summary>
    /// <see cref="http://www.hanselman.com/blog/SystemThreadingThreadCurrentPrincipalVsSystemWebHttpContextCurrentUserOrWhyFormsAuthenticationCanBeSubtle.aspx"/>
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class NapAuthAttribute : AuthorizeAttribute
    {
        #region Member

        private static readonly int _SiteId = Configuration.NapSettings.Settings.SiteId;

        private static readonly IUserService UserService = new UserService();

        #endregion Member

        #region Properties

        private string _functionKey;

        private Constant.Policy _policy;

        public string FunctionKey
        {
            get
            {
                return this._functionKey == null ? string.Empty : this._functionKey;
            }

            set
            {
                this._functionKey = value;
            }
        }

        public Constant.Policy Policy
        {
            get
            {
                return this._policy == Constant.Policy.None ? Constant.Policy.Read : this._policy;
            }

            set
            {
                this._policy = value;
            }
        }

        #endregion Properties

        /// <summary>
        /// Authorize action handler.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext == null)
            {
                this.CloseConnection(filterContext);
                return;
            }

            // why it is null?
            var principal = filterContext.HttpContext.User as NapPrincipal;

            if (principal == null)
            {
                principal = new NapPrincipal(filterContext.HttpContext);
            }

            if (object.ReferenceEquals(principal, null) || object.ReferenceEquals(principal.AuthInfo, null))
            {
                this.CloseConnection(filterContext);
                return;
            }

            if (CheckFunctionAuthorization(principal.AuthInfo.UserId))
            {
                // allow access
                // log access function here
                // write log function here (pending due to performance)
            }
            else
            {
                this.CloseConnection(filterContext);
                return;
            }
        }

        /// <summary>
        /// Check function authorization.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>True : has permission, false : not.</returns>
        public bool CheckFunctionAuthorization(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }

            int policy = UserService.GetUserSiteFunctionKeyPolicy(userId, _SiteId, this.FunctionKey);

            return policy >= (int)this.Policy;
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        private void CloseConnection(AuthorizationContext filterContext)
        {
            if (filterContext == null
                || filterContext.HttpContext == null
                || filterContext.HttpContext.Request == null)
            {
                this.SignOut();
                return;
            }

            HttpResponseBase response = filterContext.HttpContext.Response;

            if (response == null)
            {
                this.SignOut();
                return;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new HttpStatusCodeResult(403, "<center><b>" + ViewResources.UnauthorizeMsg + "</b></center>");

                response.Buffer = true;
                response.StatusCode = 403; // Unauthorized status code
                response.Write("<center><b>" + ViewResources.UnauthorizeMsg + "</b></center>");
                response.Flush();
                response.Close();
                response.End();
            }
            else if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                this.SignOut();
            }
            else
            {
                string settingUrl = NapSettings.Settings.UnauthorizeUrl;
                filterContext.Result = new RedirectResult(string.IsNullOrWhiteSpace(settingUrl) ? "~/Admin/Common/Unauthorize" : settingUrl);
            }
        }

        private void SignOut()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}