// --------------------------------------------------------------------------
// <copyright file="IMaintenanceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Lewis.Le" date="2012.10.29">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Interface
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NAP.Core.Model.ValueObject;

    public interface IMaintenanceService : IBaseService
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        Downtime GetDowntimeInfo();
    }
}