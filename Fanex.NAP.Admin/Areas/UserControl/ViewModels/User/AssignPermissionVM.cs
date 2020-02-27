namespace Fanex.NAP.Admin.Areas.UserControl.ViewModels.User
{
    using NAP.Core.Criteria.User;
    using NAP.Core.Domain;
    using NAP.Core.Model.ValueObject;
    using System.Collections.Generic;
    using System.Web.Mvc;

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