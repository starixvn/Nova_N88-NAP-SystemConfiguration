// --------------------------------------------------------------------------
// <copyright file="DomainSetting.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2013.1.3">Created</change>
// </history>

namespace Fanex.NAP.Core.Configuration
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class DomainSetting : ConfigurationSection
    {
        private const string __NAP_ADD = "domain";
        private const string __NAP_DOMAIN = "DomainSetting";

        /// <summary>
        /// List domnain
        /// </summary>        
        [ConfigurationProperty(__NAP_DOMAIN, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DomainCollection),
            AddItemName = __NAP_ADD)]
        public DomainCollection Domain
        {
            get
            {
                var domainCollection = (DomainCollection)base[__NAP_DOMAIN];

                return domainCollection;
            }
        }

        private static DomainSetting _settings = ConfigurationManager.GetSection(__NAP_DOMAIN) as DomainSetting;

        public static DomainSetting Settings
        {
            get
            {
                return _settings;
            }
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class DomainCollection : ConfigurationElementCollection
    {
        public DomainCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public void Add(DomainElement mappingElement)
        {
            BaseAdd(mappingElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DomainElement();
        }

        /// <summary>
        /// Get Element Key
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainElement)element).Name + ((DomainElement)element).Path;
        }

        /// <summary>
        /// Get Element
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DomainElement this[int index]
        {
            get
            {
                return (DomainElement)BaseGet(index);
            }

            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class DomainElement : ConfigurationElement
    {
        private const string __NAP_DOMAIN = "name";
        private const string __NAP_PATH = "path";

        [ConfigurationProperty(__NAP_DOMAIN, IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this[__NAP_DOMAIN];
            }

            set
            {
                this[__NAP_DOMAIN] = value;
            }
        }

        [ConfigurationProperty(__NAP_PATH, IsRequired = false, IsKey = true)]
        public string Path
        {
            get
            {
                return (string)this[__NAP_PATH];
            }

            set
            {
                this[__NAP_PATH] = value;
            }
        }
    }
}