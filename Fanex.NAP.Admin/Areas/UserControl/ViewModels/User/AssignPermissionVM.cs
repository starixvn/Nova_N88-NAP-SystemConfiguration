namespace Fanex.NAP.Admin.Areas.UserControl.ViewModels.User
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using N88.Core.Criteria.User;
    using N88.Core.Domain;
    using N88.Core.Model.ValueObject;

    public class AssignPermissionVM
    {
        public ReturnData ReturnData { get; set; } = new ReturnData();

        public AssignFunctionUserCriteria Criteria { get; set; }

        public List<FunctionPolicy> GroupFuncPolicyList { get; set; }

        public List<SelectListItem> Sites { get; set; }

        public List<IFunction> FunctionList { get; set; }

        public List<FunctionPolicy> UserFuncPolicyList { get; set; }

        public List<FunctionPolicy> CurrentUserFuncPolicyList { get; set; }

        public int AdminLevel { get; set; }

        public int UserLevel { get; set; }
    }
}