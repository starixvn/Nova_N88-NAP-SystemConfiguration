// --------------------------------------------------------------------------
// <copyright file="MaintenanceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Lewis.Le" date="2012.10.29">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Impl
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Service.Interface;
    using Sunplus.UMControllerLibrary;

    /// <summary>
    /// Maintenance service.
    /// </summary>
    public class MaintenanceService : BaseService, IMaintenanceService
    {
        /// <summary>
        /// Get down time information.
        /// </summary>
        /// <returns>Down time object.</returns>
        public Downtime GetDowntimeInfo()
        {
            var downTime = new Downtime
            {
                From = "0:00",
                To = "0:00",
                IsDowntime = false
            };

            if (NapSettings.Settings.EnableCheckUM)
            {
                string timeFrom = string.Empty;
                string timeTo = string.Empty;
                var umctrl = new UMBusiness();

                // gets Agent Site UM Status.
                downTime.IsDowntime = umctrl.isUnderMaintainence(
                    Sites.AccountSite,
                    NapSettings.Settings.UMAddress,
                    ref timeFrom,
                    ref timeTo);

                if (downTime.IsDowntime)
                {
                    downTime.From = timeFrom;
                    downTime.To = timeTo;
                }
            }

            return downTime;
        }
    }
}