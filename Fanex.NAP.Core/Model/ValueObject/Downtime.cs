// --------------------------------------------------------------------------
// <copyright file="Downtime.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Lewis.Le" date="2012.10.29">Created</change>
// </history>

namespace Fanex.NAP.Core.Model.ValueObject
{
    /// <summary>
    /// Downtime model.
    /// </summary>
    public class Downtime
    {
        public string From { get; set; }

        public string To { get; set; }

        public bool IsDowntime { get; set; }
    }
}