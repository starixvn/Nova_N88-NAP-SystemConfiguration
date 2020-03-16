namespace Fanex.N88.Admin.Areas.UserControl.ViewModels.User
{
    public class ReturnData
    {
        public string ReturnUserName { get; set; } = string.Empty;

        public int ReturnSiteId { get; set; }

        public int ReturnGroupId { get; set; }

        public int ReturnPageIndex { get; set; } = 1;
    }
}