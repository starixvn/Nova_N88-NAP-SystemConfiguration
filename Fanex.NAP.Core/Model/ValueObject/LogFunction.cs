// --------------------------------------------------------------------------
// <copyright file="LogFunction.cs" company="Nexcel Solutions Vietnam">
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
    /// Log function model.
    /// </summary>
    public class LogFunction
    {
        public int Id { get; set; }
               
        public string IP { get; set; }

        public DateTime LogDate { get; set; }

        public string BrowserName { get; set; }

        public string Params { get; set; }

        public Function Function { get; set; }

        public User User { get; set; }

        public Site Site { get; set; }
    }
}
