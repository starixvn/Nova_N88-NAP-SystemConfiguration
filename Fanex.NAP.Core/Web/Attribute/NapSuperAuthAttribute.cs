// --------------------------------------------------------------------------
// <copyright file="NapSuperAuthAttribute.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.2">Created</change>
// </history>

namespace Fanex.NAP.Core.Web.Attribute
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NAP.Core.Web.Session;

    /// <summary>
    /// Super authorize attribute.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class NapSuperAuthAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Authorize action handler.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext == null)
            {
                return;
            }

            if (!CheckSessionAuthorization())
            {
                HttpResponseBase response = filterContext.HttpContext.Response;

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    response.Buffer = true;
                    response.StatusCode = 403; // Unauthorized status code
                    response.Write("<center><b>" + ViewResources.UnauthorizeMsg + "</b></center>");
                    response.Flush();
                    response.Close();
                    response.End();
                }
                else
                {
                    var loginUrl = !string.IsNullOrWhiteSpace(FormsAuthentication.LoginUrl) ? FormsAuthentication.LoginUrl : "~/userlogin/login";
                    response.Redirect(loginUrl, false);
                }
            }
        }

        private bool CheckSessionAuthorization()
        {
            ICurrentSession session = new CurrentSession();

            if (session == null || string.IsNullOrEmpty(session.Username) || session.UserId <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
