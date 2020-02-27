// --------------------------------------------------------------------------
// <copyright file="Constant.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.8.8">Created</change>
// </history>

namespace Fanex.NAP.Core.Business
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The constant class.
    /// </summary>
    public static class Constant
    {
        public const string Space = " ";

        public const string Ok = "OK";

        public const string Error = "ERROR";

        public const string Uat = "UAT";

        public const string Live = "LIVE";

        public const string SiteId = "SiteId";

        public const string Environment = "Environment";

        public const int SearchAllSite = -9999;

        public const string IT = "IT";

        /// <summary>
        /// The user policy.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Reviewed")]
        public enum Policy
        {
            /// <summary>
            /// The none permission.
            /// </summary>
            None = 0,

            /// <summary>
            /// The read permission.
            /// </summary>
            Read = 1,

            /// <summary>
            /// The host permission.
            /// </summary>
            Host = 2
        }

        /// <summary>
        /// Crud permission.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Reviewed")]
        public enum Crud
        {
            /// <summary>
            /// The read permission.
            /// </summary>
            Read = 0,

            /// <summary>
            /// The host permission.
            /// </summary>
            Host = 1
        }

        /// <summary>
        /// User level.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Reviewed")]
        public enum Level
        {
            /// <summary>
            /// The user level.
            /// </summary>
            User = 0,

            /// <summary>
            /// The site admin.
            /// </summary>
            SiteAdmin = 50,

            /// <summary>
            /// The portal admin.
            /// </summary>
            PortalAdmin = 100
        }

        /// <summary>
        /// User level.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Reviewed")]
        public enum ReCheckInResult
        {
            /// <summary>
            /// Recheck in ok.
            /// </summary>
            Ok = 0,

            /// <summary>
            /// Downtime time.
            /// </summary>
            DownTime = 1,

            /// <summary>
            /// User is kicked out.
            /// </summary>
            KickedOut = 2
        }
    }
}