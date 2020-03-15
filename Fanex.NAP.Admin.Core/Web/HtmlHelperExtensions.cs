namespace Fanex.NAP.Admin.Core.Web
{
    using System;
    using System.Globalization;
    using System.Web;
    using Fanex.N88.Core.Business;
    using Fanex.N88.Core.Web;

    /// <summary>
    /// HTML Helpers.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Converts the value of the current System.DateTime object to its equivalent string
        /// representation using the specified format.
        /// </summary>
        /// <param name="value">Date time value.</param>
        /// <param name="dateFormat">Date format.</param>
        /// <param name="timeFormat">Time format.</param>
        /// <returns>HTML string.</returns>
        public static IHtmlString ToFullDateString(
            this DateTime value,
            string dateFormat = "MMM dd, yyyy",
            string timeFormat = "HH:mm tt")
        {
            return new HtmlString(
                string.Format(
                    "<span class='datetime'><span class='date'>{0}</span> <span class='time'>{1}</span></span>",
                    value.ToString(dateFormat),
                    value.ToString(timeFormat)));
        }

        /// <summary>
        /// Converts the value of the user level to its equivalent string representation.
        /// </summary>
        /// <param name="value">User level value.</param>
        /// <returns>HTML string.</returns>
        public static IHtmlString ToUserLevel(this int value)
        {
            switch (value)
            {
                case (int)Constant.Level.PortalAdmin:
                    return new HtmlString(ViewResources.Portal_Admin);

                case (int)Constant.Level.SiteAdmin:
                    return new HtmlString(ViewResources.Site_Admin);

                case (int)Constant.Level.User:
                    return new HtmlString(ViewResources.User);

                default:
                    return new HtmlString(string.Empty);
            }
        }

        /// <summary>
        /// Convert the value of the policy to string.
        /// </summary>
        /// <param name="value">Policy value.</param>
        /// <returns>HTML string.</returns>
        public static IHtmlString ToPolicy(this int value)
        {
            switch (value)
            {
                case (int)Constant.Policy.Host:
                    return new HtmlString(ViewResources.Host);

                case (int)Constant.Policy.Read:
                    return new HtmlString(ViewResources.Read);

                default:
                    return new HtmlString(ViewResources.None);
            }
        }

        /// <summary>
        /// Decode xml of [f_EncodeXMLValue].
        /// </summary>
        /// <param name="value">Encoded XML value.</param>
        /// <returns>XML value.</returns>
        public static string DecodeXMLValue(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value.Replace("&amp;", "&")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    .Replace("&quot;", "\"")
                    .Replace("&apos;", "''");
            }

            return value;
        }

        /// <summary>
        /// Get location name.
        /// </summary>
        /// <param name="geoId">Geographical ID.</param>
        /// <returns>Geographical name.</returns>
        public static string GetGeoName(this string geoId)
        {
            var listCulture = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in listCulture)
            {
                var region = new RegionInfo(culture.LCID);

                if (region.GeoId.ToString().Equals(geoId))
                {
                    return region.DisplayName;
                }
            }

            return geoId;
        }

        /// <summary>
        /// Get operating system form user agent string.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <returns>Operating system.</returns>
        public static string GetOperatingSystem(this string platform)
        {
            string[] array = UserAgentTools.GetOS(platform);
            return array != null ? array[1] : platform;
        }
    }
}