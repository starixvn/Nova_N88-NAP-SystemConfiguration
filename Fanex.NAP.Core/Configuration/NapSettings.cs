// --------------------------------------------------------------------------
// <copyright file="NapSettings.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Lewis.Le" date="2012.10.17">Created</change>
// </history>

namespace Fanex.NAP.Core.Configuration
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Custom configuration for NAP.
    /// </summary>
    public class NapSettings : ConfigurationSection
    {
        #region consts

        private const string __NAP_SETTINGS = "NapSettings";
        private const string __ENVIRONMENT = "Environment";
        private const string __ENABLE_DIRECT_LOGIN = "EnableDirectLogin";
        private const string __HUB_URL = "HubUrl";
        private const string __SITE_ID = "SiteId";
        private const string __UM_ADDRESS = "UMAddress";
        private const string __ENABLE_CHECK_UM = "EnableCheckUM";
        private const string __ENABLE_RECHECKIN = "EnableReCheckin";
        private const string __UNAUTHORIZE_URL = "UnauthorizeUrl";
        private const string __NAP_DOMAIN = "Domain";
        private const string __NAP_DOMAIN__ADD = "add";
        private const string NAPUseWindowsAuthentication = "UseWindowsAuthentication";

        #endregion consts

        private static NapSettings _settings = ConfigurationManager.GetSection(__NAP_SETTINGS) as NapSettings;

        public static NapSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public bool IsNapHub
        {
            get
            {
                return SiteId == 0;
            }
        }

        [ConfigurationProperty(__SITE_ID, IsRequired = true)]
        public int SiteId
        {
            get { return (int)this[__SITE_ID]; }
            set { this[__SITE_ID] = value; }
        }

        public bool EnableDirectLogin
        {
            get
            {
                var result = false;

                if (!bool.TryParse(_EnableDirectLogin, out result))
                {
                    result = Equals(_EnableDirectLogin, "1");
                }

                return result;
            }

            set
            {
                _EnableDirectLogin = value.ToString();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Reviewed")]
        [ConfigurationProperty(__HUB_URL, IsRequired = true)]
        public string HubUrl
        {
            get { return (string)this[__HUB_URL]; }
            set { this[__HUB_URL] = value; }
        }

        [ConfigurationProperty(__ENVIRONMENT, IsRequired = true)]
        public string Environment
        {
            get { return (string)this[__ENVIRONMENT]; }
            set { this[__ENVIRONMENT] = value; }
        }

        [ConfigurationProperty(__UM_ADDRESS, IsRequired = true)]
        public string UMAddress
        {
            get { return (string)this[__UM_ADDRESS]; }
            set { this[__UM_ADDRESS] = value; }
        }

        public bool EnableCheckUM
        {
            get
            {
                var result = false;

                if (!bool.TryParse(this._EnableCheckUM, out result))
                {
                    result = Equals(this._EnableCheckUM, "1");
                }

                return result;
            }

            set
            {
                this._EnableCheckUM = value.ToString();
            }
        }

        public bool EnableReCheckIn
        {
            get
            {
                var result = false;

                if (!bool.TryParse(_EnableReCheckIn, out result))
                {
                    result = Equals(_EnableReCheckIn, "1");
                }

                return result;
            }

            set
            {
                _EnableReCheckIn = value.ToString();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Reviewed")]
        [ConfigurationProperty(__UNAUTHORIZE_URL, IsRequired = false)]
        public string UnauthorizeUrl
        {
            get { return (string)this[__UNAUTHORIZE_URL]; }
            set { this[__UNAUTHORIZE_URL] = value; }
        }

        [ConfigurationProperty(__NAP_DOMAIN, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DomainCollection), AddItemName = __NAP_DOMAIN__ADD)]
        public DomainCollection Domains
        {
            get
            {
                var domainCollection = (DomainCollection)base[__NAP_DOMAIN];

                return domainCollection;
            }
        }

        [ConfigurationProperty(NAPUseWindowsAuthentication, IsRequired = false, DefaultValue = true)]
        public bool UseWindowsAuthentication
        {
            get { return (bool)this[NAPUseWindowsAuthentication]; }
            set { this[NAPUseWindowsAuthentication] = value; }
        }

        [ConfigurationProperty(__ENABLE_DIRECT_LOGIN, IsRequired = true)]
        private string _EnableDirectLogin
        {
            get { return (string)this[__ENABLE_DIRECT_LOGIN]; }
            set { this[__ENABLE_DIRECT_LOGIN] = value; }
        }

        [ConfigurationProperty(__ENABLE_CHECK_UM, IsRequired = true)]
        private string _EnableCheckUM
        {
            get { return (string)this[__ENABLE_CHECK_UM]; }
            set { this[__ENABLE_CHECK_UM] = value; }
        }

        [ConfigurationProperty(__ENABLE_RECHECKIN, IsRequired = true)]
        private string _EnableReCheckIn
        {
            get { return (string)this[__ENABLE_RECHECKIN]; }
            set { this[__ENABLE_RECHECKIN] = value; }
        }
    }
}