// --------------------------------------------------------------------------
// <copyright file="ICommandResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.6">Created</change>
// </history>

namespace Fanex.NAP.Core.Command
{
    public interface ICommandResult
    {
        dynamic Code { get; }

        dynamic Message { get; }

        bool IsSuccessful { get; }
    }
}
