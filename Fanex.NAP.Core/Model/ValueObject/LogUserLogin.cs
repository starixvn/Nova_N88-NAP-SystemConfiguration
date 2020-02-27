// --------------------------------------------------------------------------
// <copyright file="LogUserLogin.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.11.14">Created</change>
// </history>

namespace Fanex.NAP.Core.Model.ValueObject
{
    using System;

    /// <summary>
    /// Log user login model.
    /// </summary>
    public class LogUserLogin
    {
        public long Id { get; set; }

        public string IP { get; set; }

        public DateTime LogDate { get; set; }

        public string Doing { get; set; }

        public string DomainName { get; set; }

        public string BrowserName { get; set; }

        public string Platform { get; set; }

        public string CountryCode { get; set; }

        public int Duration { get; set; }

        public User User { get; set; }

        public Site Site { get; set; }
    }
}
