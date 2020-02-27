// --------------------------------------------------------------------------
// <copyright file="LogExecSp.cs" company="Nexcel Solutions Vietnam">
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
    /// Log execute SP model.
    /// </summary>
    public class LogExecSp
    {
        public long Id { get; set; }

        public DateTime LogDate { get; set; }

        public string Name { get; set; }

        public string Modifier { get; set; }

        public string ActionName { get; set; }

        public short ActionId { get; set; }

        public string OldData { get; set; }

        public string Data { get; set; }

        public int LogType { get; set; }

        public string Items { get; set; }

        public string SubItems { get; set; }
    }
}
