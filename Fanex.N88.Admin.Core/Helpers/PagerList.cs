namespace Fanex.N88.Core.Helpers
{
    using System.Collections.Generic;
    using Webdiyer.WebControls.Mvc;

    /// <summary>
    /// Page list.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    public class PagerList<T> : List<T>, IPagedList
    {
        public PagerList(int pageIndex, int pageSize, int totalItemCount, IEnumerable<T> items)
        {
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
            AddRange(items);
        }

        public int CurrentPageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalItemCount { get; set; }
    }
}