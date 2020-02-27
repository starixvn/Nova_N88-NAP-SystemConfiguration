// --------------------------------------------------------------------------
// <copyright file="CommandResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kos.Nguyen" date="2012.8.14">Created</change>
// </history>

namespace Fanex.NAP.Core.Command
{
    /// <summary>
    /// Command result.
    /// </summary>
    public class CommandResult : CommandResultBase
    {
        public override bool IsSuccessful
        {
            get { return this.Code == 0; }
        }
    }
}
