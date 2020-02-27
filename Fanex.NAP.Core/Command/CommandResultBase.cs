// --------------------------------------------------------------------------
// <copyright file="CommandResultBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.8.6">Created</change>
// </history>

namespace Fanex.NAP.Core.Command
{
    using System;

    /// <summary>
    /// Command result base.
    /// </summary>
    public class CommandResultBase : ICommandResult
    {
        private dynamic _code;
        private dynamic _message;

        public CommandResultBase()
            : this(string.Empty, string.Empty)
        { 
        }

        public CommandResultBase(dynamic code, dynamic message)
        {
            this._code = code;
            this._message = message;
        }

        public dynamic Code
        {
            get { return this._code; }
            set { this._code = value; }
        }

        public dynamic Message
        {
            get { return this._message; }
            set { this._message = value; }
        }

        public virtual bool IsSuccessful
        {
            get { return true; }
        }
    }
}
