namespace Fanex.NAP.Core.Criteria.Site
{
    using Fanex.Data.Repository;

    public class GetBySiteKeyCriteria : CriteriaBase
    {
        public string SiteKey { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(SiteKey);
        }

        public override string GetSettingKey()
        {
            return "NAP_Site_GetSiteKey";
        }
    }
}
