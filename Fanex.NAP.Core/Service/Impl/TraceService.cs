// --------------------------------------------------------------------------
// <copyright file="TraceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.11.14">Created</change>
// </history>

namespace Fanex.NAP.Core.Service.Impl
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml.Linq;
    using Fanex.NAP.Core.Criteria.Trace;
    using Fanex.NAP.Core.Model.ValueObject;
    using Fanex.NAP.Core.Repository.Impl;
    using Fanex.NAP.Core.Repository.Interface;
    using Fanex.NAP.Core.Service.Interface;

    /// <summary>
    /// Trace service.
    /// </summary>
    public class TraceService : BaseService, ITraceService
    {
        private readonly ITraceRepository _traceRepository;

        public TraceService()
            : this(null)
        {
        }

        public TraceService(ITraceRepository traceRepository)
        {
            this._traceRepository = traceRepository ?? new TraceRepository();
        }

        /// <summary>
        /// Get execute SP log by ID.
        /// </summary>
        /// <param name="logId">Execute SP log ID.</param>
        /// <returns>Execute SP log.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed")]
        public LogExecSp GetById(int logId)
        {
            var criteria = new GetByIdLogCriteria
            {
                LogId = logId
            };

            if (criteria.IsValid())
            {
                LogExecSp logExec = this._traceRepository.GetById(criteria);

                if (!object.ReferenceEquals(logExec, null))
                {
                    var hashItems = new Hashtable();
                    var hashSubItems = new Hashtable();

                    if (!string.IsNullOrWhiteSpace(logExec.Items))
                    {
                        XDocument xmlItems = XDocument.Parse(logExec.Items);
                        IEnumerable<XElement> xmlItemRows = xmlItems.Descendants("row");

                        foreach (XElement item in xmlItemRows)
                        {
                            string alias = item.Attribute("alias").Value;
                            if (!hashItems.Contains(alias))
                            {
                                hashItems.Add(alias, item.Attribute("item").Value);
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(logExec.SubItems))
                    {
                        XDocument xmlItems = XDocument.Parse(logExec.SubItems);
                        IEnumerable<XElement> xmlItemRows = xmlItems.Descendants("row");

                        foreach (XElement item in xmlItemRows)
                        {
                            string alias = item.Attribute("alias").Value;
                            if (!hashSubItems.Contains(alias))
                            {
                                hashSubItems.Add(alias, item.Attribute("item").Value);
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(logExec.Data))
                    {
                        XDocument xmlData = XDocument.Parse(logExec.Data);
                        var xmlElementData = new XElement("ROOT");

                        if (hashItems.Count > 0)
                        {
                            foreach (DictionaryEntry item in hashItems)
                            {
                                XAttribute attribute = xmlData.Root.Attribute(item.Key.ToString());
                                string value = attribute != null ? attribute.Value : string.Empty;

                                xmlElementData.SetAttributeValue(item.Value.ToString(), value);
                            }
                        }

                        if (hashSubItems.Count > 0)
                        {
                            var xmlElementValue = new XElement("ROW");
                            IEnumerable<XElement> xmlElementSubData = xmlData.Descendants("z");

                            foreach (XElement item in xmlElementSubData)
                            {
                                var xmlElementChildTemp = new XElement("ITEM");

                                foreach (DictionaryEntry itemAtt in hashSubItems)
                                {
                                    XAttribute attribute = item.Attribute(itemAtt.Key.ToString());
                                    string value = attribute != null ? attribute.Value : string.Empty;

                                    xmlElementChildTemp.SetAttributeValue(itemAtt.Value.ToString(), value);
                                }

                                xmlElementValue.Add(xmlElementChildTemp);
                            }

                            xmlElementData.SetAttributeValue("Value", this.EncodeXMLValue(xmlElementValue.ToString()));
                        }

                        logExec.Data = xmlElementData.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(logExec.OldData))
                    {
                        XDocument xmlData = XDocument.Parse(logExec.OldData);
                        var xmlElementData = new XElement("ROOT");

                        if (hashItems.Count > 0)
                        {
                            foreach (DictionaryEntry item in hashItems)
                            {
                                XAttribute attribute = xmlData.Root.Attribute(item.Key.ToString());
                                string value = attribute != null ? attribute.Value : string.Empty;

                                xmlElementData.SetAttributeValue(item.Value.ToString(), value);
                            }
                        }

                        if (hashSubItems.Count > 0)
                        {
                            var xmlElementValue = new XElement("ROW");
                            IEnumerable<XElement> xmlElementSubData = xmlData.Descendants("z");

                            foreach (XElement item in xmlElementSubData)
                            {
                                var xmlElementChildTemp = new XElement("ITEM");
                                foreach (DictionaryEntry itemAtt in hashSubItems)
                                {
                                    XAttribute attribute = item.Attribute(itemAtt.Key.ToString());
                                    var value = attribute != null ? attribute.Value : string.Empty;

                                    xmlElementChildTemp.SetAttributeValue(itemAtt.Value.ToString(), value);
                                }

                                xmlElementValue.Add(xmlElementChildTemp);
                            }

                            xmlElementData.SetAttributeValue("Value", this.EncodeXMLValue(xmlElementValue.ToString()));
                        }

                        logExec.OldData = xmlElementData.ToString();
                    }
                }

                return logExec;
            }

            return null;
        }

        /// <summary>
        /// Search execute SP log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of execute SP log.</returns>
        public IQueryable<LogExecSp> SearchExecSp(GetAllLogCriteria criteria)
        {
            if (criteria.IsValid())
            {
                return this._traceRepository.SearchLogExecSp(criteria);
            }

            return null;
        }

        /// <summary>
        /// Search user login log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of user login log.</returns>
        public IQueryable<LogUserLogin> SearchUserLogin(GetAllLogUserLoginCriteria criteria)
        {
            if (criteria.IsValid())
            {
                return this._traceRepository.SearchLogUserLogin(criteria);
            }

            return null;
        }

        /// <summary>
        /// Search function log.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of function log.</returns>
        public IQueryable<LogFunction> SearchLogFunction(GetAllLogFunctionCriteria criteria)
        {
            if (criteria.IsValid())
            {
                return this._traceRepository.SearchLogFunction(criteria);
            }

            return null;
        }

        /// <summary>
        /// Decode xml of [f_EncodeXMLValue].
        /// </summary>
        /// <param name="value">XML value.</param>
        /// <returns>Encoded XML value.</returns>
        private string EncodeXMLValue(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("&", "&amp;");
                value = value.Replace("<", "&lt;");
                value = value.Replace(">", "&gt;");
                value = value.Replace("\"", "&quot;");
                value = value.Replace("''", "&apos;");

                return value;
            }

            return value;
        }
    }
}