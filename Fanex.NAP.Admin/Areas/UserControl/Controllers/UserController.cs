namespace Fanex.NAP.Admin.Areas.UserControl.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Common;
    using Common.Extensions;
    using Core.Configuration;
    using Fanex.NAP.Admin.Configurations;
    using NAP.Core.Business;
    using NAP.Core.Command;
    using NAP.Core.Criteria.User;
    using NAP.Core.Domain;
    using NAP.Core.Helpers;
    using NAP.Core.Model.ValueObject;
    using NAP.Core.Service.Impl;
    using NAP.Core.Service.Interface;
    using NAP.Core.Web;
    using NAP.Core.Web.Attribute;
    using ViewModels.User;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed")]
    [NapSuperAuth]
    public class UserController : BaseController
    {
        #region Member

        private const string FormTokenKey = "_SecureChangePasswordFormToken";

        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly ISiteService _siteService;
        private readonly IFunctionService _functionService;
        private readonly HashAlgorithm _hashAlgorithm;

        #endregion Member

        #region Constructors

        public UserController()
            : this(null, null, null, null)
        {
        }

        public UserController(
            IUserService userService,
            IGroupService groupService,
            ISiteService siteService,
            IFunctionService functionService)
        {
            _userService = userService ?? new UserService { ServiceContext = ServiceContext };
            _groupService = groupService ?? new GroupService { ServiceContext = ServiceContext };
            _siteService = siteService ?? new SiteService { ServiceContext = ServiceContext };
            _functionService = functionService ?? new FunctionService { ServiceContext = ServiceContext };
            _hashAlgorithm = new SHA256Managed();
        }

        #endregion Constructors

        #region Properties

        public string FormToken
        {
            get
            {
                if (Session[FormTokenKey] == null)
                {
                    return string.Empty;
                }

                return Session[FormTokenKey].ToString();
            }

            set
            {
                Session[FormTokenKey] = value;
            }
        }

        #endregion Properties

        #region Manage

        /// <summary>
        /// Manage user.
        /// </summary>
        /// <param name="id">The page index.</param>
        /// <param name="size">The page size.</param>
        /// <param name="userName">The username.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
#pragma warning disable S1541 // Methods and properties should not be too complex
        public ActionResult Manage(
#pragma warning restore S1541 // Methods and properties should not be too complex
            int id = 1,
            int size = 20,
            string userName = EmptyString,
            int groupId = 0,
            int siteId = -1,
            int statusId = 1)
        {
            // declares paging params
            int pageIndex = id;
            int pageSize = size;
            string username = ViewBag.Username = userName.Trim();
            var siteIdValue = siteId < 0 && siteId != Constant.SearchAllSite ? SiteId : siteId;

            // permission function manage user
            int selectedSiteId = siteId == 0 || siteId == Constant.SearchAllSite ? SiteId : siteIdValue;

            ViewFlags.HasEditRight =
                HasRight(FunctionKeys.ManageUser, Constant.Policy.Host)
                && HasRightOnSite(selectedSiteId, FunctionKeys.ManageUser, Constant.Policy.Host);

            ViewFlags.HasAssignSiteRight =
                HasRight(FunctionKeys.AssignSiteToUser, Constant.Policy.Read)
                && HasRightOnSite(selectedSiteId, FunctionKeys.AssignSiteToUser, Constant.Policy.Read);

            // if users choose a specific site (siteId == 0 means users choose [No Site])
            bool isSpecificSiteChosen = siteIdValue != 0;

            ViewFlags.IsShowLevelColumn = isSpecificSiteChosen;

            // is portal admin in NAP
            ViewBag.IsPortalAdmin = _userService.GetUserSiteLevel(CurrentSession.UserId, NapAdminId)
                                                == (int)Constant.Level.PortalAdmin;

            // detects "Assign Function" right
            if (isSpecificSiteChosen)
            {
                ViewFlags.HasAssignFunctionRight =
                    HasRight(FunctionKeys.AssignFunctionToUser, Constant.Policy.Read)
                    && HasRightOnSite(siteIdValue, FunctionKeys.AssignFunctionToUser, Constant.Policy.Read);
            }

            var listSite = new List<SelectListItem>();

            if (ThisSiteIsNapAdmin)
            {
                // gets data for dropdownlist of sites
                var siteSelectList = new List<SelectListItem>();

                listSite = UserIsPortalAdmin
                ? _siteService.GetAllSelectListItem(Environment, true, siteIdValue).ToList()
                : _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                                        CurrentSession.UserId,
                                        FunctionKeys.ManageUser,
                                        Environment,
                                        selectedSiteId: siteIdValue).ToList();

                siteSelectList.Add(new SelectListItem { Text = ViewResources.All_Site, Value = Constant.SearchAllSite.ToString() });
                siteSelectList.AddRange(listSite);
                siteSelectList.Add(new SelectListItem { Text = ViewResources.No_Site, Value = "0" });

                ViewBag.SiteSelectList = siteSelectList;
            }

            // gets data for dropdownlist of groups
            ViewBag.GroupSelectList = _groupService.GetAllSelectListItem(selectedGroupId: groupId);
            ViewBag.SiteId = siteIdValue;
            ViewBag.UserId = CurrentSession.UserId;
            ViewBag.ReturnGroupId = groupId;

            // gets level of currently-login-user
            if (isSpecificSiteChosen)
            {
                ViewBag.Level = _userService.GetUserSiteLevel(CurrentSession.UserId, siteIdValue);
            }

            if (siteIdValue == Constant.SearchAllSite)
            {
                var searchAllSiteCriteria = new GetAllByUsernameGroupSiteUserCriteria
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Username = username,
                    GroupId = groupId,
                    ListSite = listSite,
                    Status = statusId,
                    TotalRows = 0,
                    Environment = Environment
                };

                IQueryable<UserLevel> users = _userService.GetAllByUsernameGroupSite(searchAllSiteCriteria);

                var hashManageUserPolicy =
                    _userService.GetUserListSiteFunctionKeyPolicy(CurrentSession.UserId, FunctionKeys.ManageUser, listSite);

                var hashAssignFunctionPolicy = _userService.GetUserListSiteFunctionKeyPolicy(
                                                                CurrentSession.UserId,
                                                                FunctionKeys.AssignFunctionToUser,
                                                                listSite);

                var hashAssignSite = _userService.GetUserListSiteFunctionKeyPolicy(
                                                                CurrentSession.UserId,
                                                                FunctionKeys.AssignSiteToUser,
                                                                listSite);

                var listSiteLevel = _siteService.GetSiteLevelByUser(CurrentSession.UserId, Environment, true).ToList();

                ViewBag.HashManageUser = hashManageUserPolicy;
                ViewBag.HashAssignFunction = hashAssignFunctionPolicy;
                ViewBag.HashAssignSite = hashAssignSite;
                ViewBag.ListSiteLevel = listSiteLevel;

                ViewBag.ViewFlagsCurrentSite = new ViewFlags
                {
                    HasManageUserRight = HasRight(FunctionKeys.ManageUser, Constant.Policy.Host),
                    HasAssignFunctionRight = HasRight(FunctionKeys.AssignFunctionToUser, Constant.Policy.Read),
                    HasAssignSiteRight = HasRight(FunctionKeys.AssignSiteToUser, Constant.Policy.Read)
                };

                ViewFlags.IsShowActionColumn = hashAssignFunctionPolicy.Cast<DictionaryEntry>().Any(item => (int)item.Value >= (int)Constant.Policy.Read);
                ViewFlags.IsShowActionColumn = hashAssignSite.Cast<DictionaryEntry>().Any(item => (int)item.Value >= (int)Constant.Policy.Read);
                ViewFlags.IsShowActionColumn = hashManageUserPolicy.Cast<DictionaryEntry>().Any(item => (int)item.Value >= (int)Constant.Policy.Read);

                return View(new PagerList<UserLevel>(pageIndex, pageSize, searchAllSiteCriteria.TotalRows, users));
            }
            else
            {
                var criteria = new SearchUserCriteria
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Username = username,
                    GroupId = groupId,
                    SiteId = siteIdValue,
                    Status = statusId,
                    TotalRows = 0,
                    Environment = Environment
                };

                IQueryable<UserLevel> users = _userService.SearchUser(criteria);
                ViewFlags.IsShowActionColumn = ViewFlags.HasEditRight || ViewFlags.HasAssignSiteRight
                    || ViewFlags.HasAssignFunctionRight;

                return View(new PagerList<UserLevel>(pageIndex, pageSize, criteria.TotalRows, users));
            }
        }

        /// <summary>
        /// Manage user.
        /// </summary>
        /// <param name="size">The page size.</param>
        /// <param name="userName">The username.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
        public ActionResult Manage(
            int size = 20,
            string userName = EmptyString,
            int groupId = 0,
            int siteId = -1,
            int statusId = 1)
        {
            return RedirectToAction(
                "Manage",
                new
                {
                    id = 1,
                    size,
                    userName,
                    groupId,
                    siteId,
                    statusId
                });
        }

        #endregion Manage

        #region Assign Site

        /// <summary>
        /// Assign site.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="returnSiteId">The site ID.</param>
        /// <param name="returnGroupId">The group ID.</param>
        /// <param name="returnPageIndex">The page index.</param>
        /// <returns>Action result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
        public ActionResult AssignSite(
            int id,
            string returnUserName = EmptyString,
            int returnSiteId = 0,
            int returnGroupId = 0,
            int returnPageIndex = 1)
        {
            List<ISite> siteList;
            List<ISite> editableSiteList = new List<ISite>();
            List<SiteLevel> siteLevelList;
            List<SiteLevel> currentUserSiteLevelList;

            if (UserIsPortalAdmin)
            {
                siteList = _siteService.GetAll(Environment, true).ToList();
                ViewFlags.HasAssignSiteReadRight = true;
                ViewFlags.HasAssignSiteRight = true;
            }
            else
            {
                siteList = _siteService.GetByUserAndFunctionKeyIncludeGroup(
                    CurrentSession.UserId,
                    FunctionKeys.AssignSiteToUser,
                    Environment).ToList();

                int assignSitePolicy = _userService.GetUserSiteFunctionKeyPolicy(
                    CurrentSession.UserId,
                    SiteId,
                    FunctionKeys.AssignSiteToUser);

                ViewFlags.HasAssignSiteReadRight = HasRight(FunctionKeys.AssignSiteToUser, Constant.Policy.Read);
                ViewFlags.HasAssignSiteRight = assignSitePolicy >= (int)Constant.Policy.Host;

                if (assignSitePolicy >= (int)Constant.Policy.Host)
                {
                    editableSiteList = _siteService.GetByUserAndFunctionKeyIncludeGroup(
                        CurrentSession.UserId,
                        FunctionKeys.AssignSiteToUser,
                        Environment,
                        policy: Constant.Policy.Host).ToList();
                }
            }

            currentUserSiteLevelList = _siteService.GetSiteLevelByUser(CurrentSession.UserId, Environment).ToList();
            siteLevelList = _siteService.GetSiteLevelByUser(id, Environment).ToList();

            var assignSiteVM = new AssignSiteVM
            {
                ReturnData = new ReturnData
                {
                    ReturnUserName = returnUserName,
                    ReturnSiteId = returnSiteId,
                    ReturnGroupId = returnGroupId,
                    ReturnPageIndex = returnPageIndex
                },
                SiteList = siteList,
                CurrentUserSiteLevelList = currentUserSiteLevelList,
                SiteLevelList = siteLevelList,
                EditableSiteList = editableSiteList,
                CurrentSessionLevel = CurrentSession.Level,
                UserId = id,
                UserName = _userService.GetById(id).Username,
                IsSelfEdit = CurrentSession.UserId == id,
                CurrentSiteId = SiteId,
                IsPortalAdmin = UserIsPortalAdmin
            };

            return View(assignSiteVM);
        }

        #endregion Assign Site

        #region Get Available Sites for User

        /// <summary>
        /// Get available sites for user.
        /// </summary>
        /// <param name="userId">The user identity.</param>
        /// <param name="username">The username.</param>
        /// <param name="returnSiteId">The site identity.</param>
        /// <param name="returnGroupId">The group identity.</param>
        /// <param name="returnPageIndex">The page index.</param>
        /// <param name="level">The level of current login user.</param>
        /// <returns>The list sites available for user.</returns>
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Host)]
        public ActionResult GetAvailableSitesForUser(
            int userId,
            int level,
            string username,
            string returnUserName = EmptyString,
            int returnSiteId = 0,
            int returnGroupId = 0,
            int returnPageIndex = 1)
        {
            var availableSites = new List<ISite>();
            var hostRightSites = new List<ISite>();
            var currentUserSiteLevelList = new List<SiteLevel>();

            if (CurrentSession.UserId != userId)
            {
                availableSites = _siteService.GetAvailableSitesForUser(userId, Environment).ToList();
                currentUserSiteLevelList = _siteService.GetSiteLevelByUser(CurrentSession.UserId, Environment, true).ToList();
                bool isHasAssignRight = HasRight(FunctionKeys.AssignSiteToUser, Constant.Policy.Host);

                if (isHasAssignRight)
                {
                    hostRightSites = _siteService.GetByUserAndFunctionKeyIncludeGroup(
                        CurrentSession.UserId,
                        FunctionKeys.AssignSiteToUser,
                        Environment,
                        policy: Constant.Policy.Host).ToList();
                }
            }

            var assignNewSiteVM = new AssignNewSiteVM
            {
                ReturnData = new ReturnData
                {
                    ReturnUserName = returnUserName,
                    ReturnSiteId = returnSiteId,
                    ReturnGroupId = returnGroupId,
                    ReturnPageIndex = returnPageIndex
                },
                AvailableSitesForUser = availableSites,
                CurrentUserSiteLevelList = currentUserSiteLevelList,
                HostRightSites = hostRightSites,
                UserName = username,
                UserId = userId,
                Level = level
            };

            return View("~/Areas/UserControl/Views/User/AssignNewSite.cshtml", assignNewSiteVM);
        }

        #endregion Get Available Sites for User

        #region User Info

        /// <summary>
        /// View user information.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
        public ActionResult UserInfo(int id)
        {
            return View(_userService.GetById(id));
        }

        #endregion User Info

        #region Add Existing User

        /// <summary>
        /// Add existing user.
        /// </summary>
        /// <param name="returnUserName">The username.</param>
        /// <param name="returnSiteId">The site ID.</param>
        /// <param name="returnGroupId">The group ID.</param>
        /// <param name="returnPageIndex">The return page index.</param>
        /// <returns>Action result.</returns>
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public ActionResult AddExisting(
            int id = 1,
            int size = 20,
            string userName = EmptyString,
            string returnUserName = EmptyString,
            int returnSiteId = 0,
            int returnGroupId = 0,
            int returnPageIndex = 0)
        {
            var addExistingUserVM = new AddExistingUserVM
            {
                ReturnData = new ReturnData
                {
                    ReturnUserName = returnUserName,
                    ReturnGroupId = returnGroupId,
                    ReturnSiteId = returnSiteId,
                    ReturnPageIndex = returnPageIndex
                }
            };

            var criteria = new SearchByUsernameUserCriteria
            {
                Username = userName,
                PageIndex = id,
                PageSize = size,
                TotalRows = 0
            };

            PagerList<IUser> userList = null;

            if (criteria.Username.Length > 0)
            {
                var users = _userService.SearchByUsername(criteria);

                userList = new PagerList<IUser>(criteria.PageIndex, criteria.PageSize, criteria.TotalRows, users);

                ViewBag.Groups = _groupService.GetAllSelectListItem();
                ViewBag.UserName = criteria.Username;

                if (Request.IsAjaxRequest())
                {
                    addExistingUserVM.UserList = userList;
                    return PartialView(addExistingUserVM);
                }
            }

            addExistingUserVM.UserList = userList;
            return View(addExistingUserVM);
        }

        /// <summary>
        /// Add existing user.
        /// </summary>
        /// <param name="addExistingUserVM">AddExisting View Model</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public ActionResult AddExisting(AddExistingUserVM viewModel)
        {
            return RedirectToAction(
                "AddExisting",
                new
                {
                    id = 1,
                    size = viewModel.PageSize,
                    userName = viewModel.UserName,
                    returnUserName = viewModel.ReturnData.ReturnUserName,
                    returnSiteId = viewModel.ReturnData.ReturnSiteId,
                    returnGroupId = viewModel.ReturnData.ReturnGroupId,
                    returnPageIndex = viewModel.ReturnData.ReturnPageIndex
                });
        }

        #endregion Add Existing User

        #region Add New User

        /// <summary>
        /// Add new user.
        /// </summary>
        /// <param name="returnSiteId">The site ID.</param>
        /// <param name="returnPageIndex">The page index.</param>
        /// <param name="returnGroupId">The group ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public ActionResult AddNew(
            string returnUserName = EmptyString,
            int returnSiteId = 0,
            int returnGroupId = 0,
            int returnPageIndex = 1)
        {
            var returnData = new ReturnData
            {
                ReturnUserName = returnUserName,
                ReturnSiteId = returnSiteId,
                ReturnGroupId = returnGroupId,
                ReturnPageIndex = returnPageIndex
            };

            ViewBag.Groups = _groupService.GetAllSelectListItem();
            ViewBag.UsernamePrefix = AppSettings.GetConfigValue("CoreSiteUsernamePrefix") ?? string.Empty;

            return View(new AddNewUserVM
            {
                ReturnData = returnData,
                Criteria = new InsertCustCriteria()
            });
        }

        /// <summary>
        /// Add new user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public ActionResult AddNew(AddNewUserVM addNewUserVM)
        {
            var criteria = addNewUserVM.Criteria;

            ViewBag.Groups = _groupService.GetAllSelectListItem(selectedGroupId: criteria.GroupId);

            if (ModelState.IsValid)
            {
                criteria.Username = string.Format("{0}{1}", AppSettings.GetConfigValue("CoreSiteUsernamePrefix"), criteria.Username);
                // check username duplication
                if (_userService.CheckUsernameExist(criteria.Username))
                {
                    ModelState.AddModelError("Username", string.Format(ViewResources.DuplicateMsg, criteria.Username));
                    return View(addNewUserVM);
                }

                // add new user
                criteria.UserPwd = Cryptography.CfsEnCode(criteria.UserPwd);
                criteria.FirstName = criteria.FirstName ?? string.Empty;
                criteria.LastName = criteria.LastName ?? string.Empty;
                criteria.Email = criteria.Email ?? string.Empty;

                // assign all functions to the new user if he is site admin or portal admin
                ICommandResult result = _userService.Insert(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                    ViewBag.UserId = criteria.CustId;
                    ViewBag.UsernamePrefix = AppSettings.GetConfigValue("CoreSiteUsernamePrefix") ?? string.Empty;
                }
            }

            addNewUserVM.Criteria = criteria;

            return View(addNewUserVM);
        }

        #endregion Add New User

        #region Edit User

        /// <summary>
        /// Update user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="returnSiteId">The site ID.</param>
        /// <param name="returnGroupId">Return group ID.</param>
        /// <param name="returnPageIndex">The page index.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public ActionResult Edit(
            int id,
            string returnUserName = EmptyString,
            int returnSiteId = 0,
            int returnGroupId = 0,
            int returnPageIndex = 1)
        {
            var criteria = new UpdateCustCriteria();

            IUser user = _userService.GetById(id);

            if (user == null)
            {
                return RedirectToAction("Manage");
            }

            criteria.CustId = user.Id;
            criteria.FirstName = user.FirstName;
            criteria.LastName = user.LastName;
            criteria.Email = user.Email;
            criteria.Username = user.Username;
            criteria.GroupId = user.Group != null ? user.Group.Id : 0;

            ViewBag.Groups = user.Group == null ?
                _groupService.GetAllSelectListItem() :
                _groupService.GetAllSelectListItem(selectedGroupId: user.Group.Id);

            return View(new EditUserVM
            {
                ReturnData = new ReturnData
                {
                    ReturnUserName = returnUserName,
                    ReturnSiteId = returnSiteId,
                    ReturnGroupId = returnGroupId
                },
                Criteria = criteria
            });
        }

        /// <summary>
        /// Update user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public ActionResult Edit(EditUserVM editUserVM)
        {
            var criteria = editUserVM.Criteria;

            if (ModelState.IsValid)
            {
                criteria.FirstName = criteria.FirstName ?? string.Empty;
                criteria.LastName = criteria.LastName ?? string.Empty;
                criteria.Email = criteria.Email ?? string.Empty;

                ICommandResult result = _userService.InsertOrUpdate(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            ViewBag.Groups = _groupService.GetAllSelectListItem(selectedGroupId: criteria.GroupId);

            editUserVM.Criteria = criteria;
            return View(editUserVM);
        }

        #endregion Edit User

        #region Reset Password

        /// <summary>
        /// Reset user password.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="returnSiteId">The site ID.</param>
        /// <param name="returnGroupId">The group ID.</param>
        /// <param name="returnPageIndex">The page index.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public ActionResult ResetPassword(
            int id,
            string returnUserName = EmptyString,
            int returnSiteId = 0,
            int returnGroupId = 0,
            int returnPageIndex = 1)
        {
            IUser user = _userService.GetById(id);

            if (user == null)
            {
                return RedirectToAction("Manage");
            }

            var resetPasswordVM = new ResetPasswordVM
            {
                ReturnData = new ReturnData
                {
                    ReturnUserName = returnUserName,
                    ReturnSiteId = returnSiteId,
                    ReturnGroupId = returnGroupId,
                    ReturnPageIndex = returnPageIndex
                },
                Criteria = new ResetPasswordCustCriteria
                {
                    CustId = user.Id,
                    Username = user.Username,
                    UserPwd = string.Empty,
                    ConfirmUserPwd = string.Empty,
                    Token = GenerateToken()
                }
            };

            return View(resetPasswordVM);
        }

        /// <summary>
        /// Reset user password.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public ActionResult ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            ModelState.Clear();

            var criteria = resetPasswordVM.Criteria;

            if (CheckValidToken(criteria.Token))
            {
                IUser user = _userService.GetById(criteria.CustId);

                if (user == null)
                {
                    return RedirectToAction("Manage");
                }

                ICommandResult result = _userService.InsertOrUpdate(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            criteria.Token = GenerateToken();

            resetPasswordVM.Criteria = criteria;

            return View(resetPasswordVM);
        }

        #endregion Reset Password

        #region Change Password

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var criteria = new UpdatePasswordCustCriteria
            {
                Username = CurrentSession.Username,
                Token = GenerateToken()
            };

            return View(criteria);
        }

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult ChangePassword(UpdatePasswordCustCriteria criteria)
        {
            // The password values are encoded at client side by calling CryptoJS.CfsEnCode.
            // The result of this action is the encrypted strings which totally break the password valiadtion rules.
            // They also cannot be decoded to the original ones since CryptoJS.CfsEnCode is one-way encryption algorithm.
            // Because of those reasons, so we will validate the form at client side only and not check the model state here.
            ////if (ModelState.IsValid)

            ModelState.Clear();

            if (CheckValidToken(criteria.Token))
            {
                IUser user = _userService.GetById(CurrentSession.UserId);
                string hashPassword = GetHash(user.Password + criteria.Token);

                if (hashPassword.Equals(criteria.OldUserPwd))
                {
                    criteria.CustId = user.Id;

                    ICommandResult result = _userService.InsertOrUpdate(criteria);

                    if (result != null && result.IsSuccessful)
                    {
                        ViewBag.Success = true;
                    }
                }
                else
                {
                    ModelState.AddModelError("OldUserPwd", ViewResources.Wrong_password);
                }
            }

            criteria.Token = GenerateToken();

            return View(criteria);
        }

        #endregion Change Password

        #region Assign Permission

        /// <summary>
        /// Assign user permission.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="returnPageIndex">The page index.</param>
        /// <param name="returnSiteId">Return site ID.</param>
        /// <param name="returnGroupId">The group ID.</param>
        /// <returns>Action result.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed.")]
        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Read)]
        public ActionResult AssignPermission(
            int id,
            int siteId = 0,
            string returnUserName = EmptyString,
            int returnPageIndex = 1,
            int returnSiteId = 0,
            int returnGroupId = 0)
        {
            var assignPermissionVM = new AssignPermissionVM();
            var siteIdValue = siteId == 0 ? SiteId : siteId;

            IUser user = _userService.GetById(id);
            ISite site = _siteService.GetById(siteIdValue);

            if (ReferenceEquals(user, null) || ReferenceEquals(site, null))
            {
                return HttpNotFound();
            }

            var criteria = new AssignFunctionUserCriteria
            {
                UserId = id,
                Username = user.Username,
                SiteId = siteIdValue
            };

            if (!ReferenceEquals(user.Group, null))
            {
                assignPermissionVM.GroupFuncPolicyList =
                    _functionService.GetByGroupAndSite(user.Group.Id, criteria.SiteId, Environment).ToList();
            }

            ViewFlags.HasAssignFunctionRight = HasRightOnSite(criteria.SiteId, FunctionKeys.AssignFunctionToUser, Constant.Policy.Host)
                && id != CurrentSession.UserId;

            assignPermissionVM.Sites = CurrentSession.Level == (int)Constant.Level.PortalAdmin ?
                _siteService.GetAllSelectListItem(Environment, true, criteria.SiteId).ToList() :
                _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    CurrentSession.UserId,
                    FunctionKeys.AssignFunctionToUser,
                    Environment,
                    selectedSiteId: criteria.SiteId).ToList();

            assignPermissionVM.FunctionList = _functionService.GetBySiteInHierarchy(criteria.SiteId, Environment, true).ToList();
            assignPermissionVM.UserFuncPolicyList =
                _functionService.GetByUserAndSite(criteria.UserId, criteria.SiteId, Environment).ToList();
            assignPermissionVM.CurrentUserFuncPolicyList =
                _functionService.GetByUserAndSiteIncludeGroup(CurrentSession.UserId, criteria.SiteId, Environment).ToList();
            assignPermissionVM.AdminLevel = CurrentSession.Level;
            assignPermissionVM.UserLevel = _userService.GetUserSiteLevel(user.Id, site.Id);

            assignPermissionVM.Criteria = criteria;
            assignPermissionVM.ReturnData = new ReturnData
            {
                ReturnUserName = returnUserName,
                ReturnSiteId = returnSiteId != 0 ? returnSiteId : siteIdValue,
                ReturnGroupId = returnGroupId,
                ReturnPageIndex = returnPageIndex
            };

            return View(assignPermissionVM);
        }

        /// <summary>
        /// Assign user permission.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public ActionResult AssignPermission(AssignPermissionVM assignPermissionVM)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            var criteria = assignPermissionVM.Criteria;

            // check user and site
            IUser user = _userService.GetById(criteria.UserId);
            ISite site = _siteService.GetById(criteria.SiteId);

            if (user == null || site == null)
            {
                return RedirectToAction("Manage");
            }

            // check policy
            int policy = _userService.GetUserSiteFunctionKeyPolicy(
                CurrentSession.UserId,
                criteria.SiteId,
                FunctionKeys.AssignFunctionToUser);

            if (policy < (int)Constant.Policy.Host)
            {
                return RedirectToAction("Unauthorize", "Common");
            }

            ViewFlags.HasAssignFunctionRight = policy >= (int)Constant.Policy.Host;

            criteria.Username = user.Username;
            criteria.ModifiedBy = CurrentSession.UserId;
            criteria.ModifiedDate = DateTime.Now;

            // list function policy
            List<IFunction> listFunc =
                _functionService.GetBySiteInHierarchy(criteria.SiteId, Environment, true).ToList();

            // list user function policy
            List<FunctionPolicy> listUserFuncPolicy =
                _functionService.GetByUserAndSite(criteria.UserId, criteria.SiteId, Environment).ToList();

            // list function will be remove by current user login
            var listUserFuncRemove = new List<FunctionPolicy>();
            listUserFuncRemove.AddRange(listUserFuncPolicy);

            // list current user login function policy
            List<FunctionPolicy> listCurrentUserFuncPolicy =
                _functionService.GetByUserAndSiteIncludeGroup(CurrentSession.UserId, criteria.SiteId, Environment).ToList();

            // check function host,contains in list function
            if (!criteria.FuncHost.IsNullOrDBNull())
            {
                foreach (int itemFunc in criteria.FuncHost)
                {
                    // function find from list function, policy is host
                    IFunction tempFuncHost = listFunc.FirstOrDefault(f => f.Id == itemFunc);

                    if (tempFuncHost.IsNullOrDBNull())
                    {
                        return RedirectToAction("Error", "Common");
                    }

                    // remove all function host
                    listUserFuncRemove.Remove(f => f.Function.Id == itemFunc);
                }
            }

            // check function read, with current user login have permission
            if (!criteria.FuncRead.IsNullOrDBNull())
            {
                foreach (int itemFunc in criteria.FuncRead)
                {
                    listUserFuncRemove.Remove(f => f.Function.Id == itemFunc);
                }
            }

            // check permission current user login with user be assign permission
            // check for function policy will be remove ( set policy = 0 )
            listUserFuncRemove = listUserFuncRemove.Where(f => f.Policy > 0).ToList();
            foreach (FunctionPolicy itemFunc in listUserFuncRemove)
            {
                FunctionPolicy tempFunc = listCurrentUserFuncPolicy
                                    .FirstOrDefault(f =>
                                        f.Function.Id == itemFunc.Function.Id
                                        && f.Policy >= itemFunc.Policy);

                if (tempFunc.IsNullOrDBNull())
                {
                    return RedirectToAction("Error", "Common");
                }
            }

            ICommandResult result = _userService.AssignFunction(criteria);

            if (result != null && result.IsSuccessful)
            {
                ViewBag.Success = true;

                listUserFuncPolicy =
                    _functionService.GetByUserAndSite(criteria.UserId, criteria.SiteId, Environment).ToList();
            }

            if (!ReferenceEquals(user.Group, null))
            {
                assignPermissionVM.GroupFuncPolicyList =
                    _functionService.GetByGroupAndSite(user.Group.Id, criteria.SiteId, Environment).ToList();
            }

            assignPermissionVM.Sites = CurrentSession.Level == (int)Constant.Level.PortalAdmin ?
                _siteService.GetAllSelectListItem(Environment, true, criteria.SiteId).ToList() :
                _siteService.GetSelectListItemByUserAndFunctionKeyIncludeGroup(
                    CurrentSession.UserId,
                    FunctionKeys.AssignFunctionToUser,
                    Environment,
                    selectedSiteId: criteria.SiteId).ToList();

            assignPermissionVM.FunctionList = listFunc;
            assignPermissionVM.UserFuncPolicyList = listUserFuncPolicy;
            assignPermissionVM.CurrentUserFuncPolicyList = listCurrentUserFuncPolicy;
            assignPermissionVM.AdminLevel = CurrentSession.Level;
            assignPermissionVM.UserLevel = _userService.GetUserSiteLevel(user.Id, site.Id);

            assignPermissionVM.Criteria = criteria;

            return View(assignPermissionVM);
        }

        /// <summary>
        /// Assign user permission.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public ActionResult UnassignPermission(int userId, int functionId)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            IUser user = _userService.GetById(userId);
            var function = _functionService.GetById(functionId);
            ISite site = function.Site;
            List<FunctionPolicy> listUserFuncPolicy = _functionService.GetByUserAndSite(userId, site.Id, Environment).ToList();

            var criteria = new AssignFunctionUserCriteria();
            criteria.FuncHost = listUserFuncPolicy.Where(p => p.Policy == 2 && p.Function.Id != functionId).Select(p => (int)p.Function.Id).ToArray();
            criteria.FuncRead = listUserFuncPolicy.Where(p => p.Policy == 1 && p.Function.Id != functionId).Select(p => (int)p.Function.Id).ToArray();
            criteria.ModifiedBy = CurrentSession.UserId;
            criteria.ModifiedDate = DateTime.Now;
            criteria.SiteId = site.Id;
            criteria.Username = user.Username;
            criteria.UserId = userId;

            ICommandResult result = _userService.AssignFunction(criteria);

            return Json(result);
        }

        /// <summary>
        /// Assign site to user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="level">The user level.</param>
        /// <param name="isActive">Is active or not.</param>
        /// <returns>Result string.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Host)]
        public string AssignSiteToUser(int userId, int siteId, int level, bool isActive)
        {
            var criteria = new AssignSiteUserCriteria
            {
                CreatedBy = CurrentSession.UserId,
                CreatedDate = DateTime.Now,
                UserId = userId,
                SiteId = siteId,
                Level = level,
                IsActive = isActive,
                IsNeverExpired = true,
            };

            ICommandResult result = _userService.AssignSite(criteria);

            if (result != null && result.IsSuccessful)
            {
                return Constant.Ok;
            }

            return Constant.Error;
        }

        #endregion Assign Permission

        #region Update Site To User

        /// <summary>
        /// Update information of site to user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <param name="level">The level.</param>
        /// <param name="isActive">The is active.</param>
        /// <returns>The result string.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Host)]
        public string UpdateSiteToUser(int userId, int siteId, int level, bool isActive, DateTime expiredDate, bool isNeverExpired)
        {
            // check permission again in server
            bool canUpdateSiteToUser = CheckPermissionToUpdateUser(userId, siteId, level);

            if (canUpdateSiteToUser)
            {
                var criteria = new UpdateSiteToUserCriteria
                {
                    ModifiedBy = CurrentSession.UserId,
                    ModifiedDate = DateTime.Now,
                    UserId = userId,
                    SiteId = siteId,
                    Level = level,
                    IsActive = isActive,
                    ExpiredDate = expiredDate,
                    IsNeverExpired = isNeverExpired
                };

                ICommandResult result = _userService.UpdateSite(criteria);

                if (result != null && result.IsSuccessful)
                {
                    return Constant.Ok;
                }
            }

            return Constant.Error;
        }

        #endregion Update Site To User

        #region Profile

        /// <summary>
        /// View user profile.
        /// </summary>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult UserProfile()
        {
            var criteria = new UpdateCustCriteria();
            var user = _userService.GetById(CurrentSession.UserId);

            criteria.CustId = user.Id;
            criteria.Username = user.Username;
            criteria.FirstName = user.FirstName;
            criteria.LastName = user.LastName;
            criteria.Email = user.Email;
            criteria.GroupId = user.Group != null ? user.Group.Id : 0;

            return View(criteria);
        }

        /// <summary>
        /// View user profile.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult UserProfile(UpdateCustCriteria criteria)
        {
            if (ModelState.IsValid)
            {
                criteria.CustId = CurrentSession.UserId;
                criteria.Username = CurrentSession.Username;
                criteria.FirstName = criteria.FirstName ?? string.Empty;
                criteria.LastName = criteria.LastName ?? string.Empty;
                criteria.Email = criteria.Email ?? string.Empty;

                ICommandResult result = _userService.InsertOrUpdate(criteria);

                if (result != null && result.IsSuccessful)
                {
                    ViewBag.Success = true;
                }
            }

            return View(criteria);
        }

        #endregion Profile

        #region Assign Site/Function To User

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
        [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Read)]
        public ActionResult AssignSiteFunction(
         int id,
         string returnUserName = EmptyString,
         int returnSiteId = 0,
         int returnGroupId = 0,
         int returnPageIndex = 1)
        {
            var loginUserAssignSitePolicy = _userService.GetUserSiteFunctionKeyPolicy(
                    CurrentSession.UserId,
                    SiteId,
                    FunctionKeys.AssignSiteToUser);

            var loginUserAssignFunctionPolicy = _userService.GetUserSiteFunctionKeyPolicy(
                    CurrentSession.UserId,
                    SiteId,
                    FunctionKeys.AssignFunctionToUser);

            return View(new AssignSiteFunctionViewModel
            {
                LoginUserId = CurrentSession.UserId,
                UserName = _userService.GetById(id).Username,
                UserId = id,
                LoginUserIsPortalAdmin = UserIsPortalAdmin,
                LoginUserHasAssignSitePermission = loginUserAssignSitePolicy >= (int)Constant.Policy.Host,
                LoginUserHasAssignSiteReadPermission = loginUserAssignSitePolicy == (int)Constant.Policy.Read,
                LoginUserAssignFunctionPermission = loginUserAssignFunctionPolicy,
                IsSelfEdit = CurrentSession.UserId == id,
                CurrentSiteId = SiteId,
                ReturnData = new ReturnData
                {
                    ReturnUserName = returnUserName,
                    ReturnGroupId = returnGroupId,
                    ReturnPageIndex = returnPageIndex,
                    ReturnSiteId = returnSiteId
                }
            });
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Host)]
        public ActionResult AssignFunction(List<AssignFunctionUserCriteria> payload)
        {
            foreach (var data in payload)
            {
                IUser user = _userService.GetById(data.UserId);
                ISite site = _siteService.GetById(data.SiteId);

                if (user == null || site == null)
                {
                    return RedirectToAction("Manage");
                }

                data.Username = user.Username;
                data.ModifiedBy = CurrentSession.UserId;
                data.ModifiedDate = DateTime.Now;

                var result = _userService.AssignFunction(data);

                if (result != null && result.IsSuccessful)
                {
                    return Json(Constant.Ok);
                }
            }

            return Json(Constant.Error);
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
        public JsonResult GetAllUserSites(int userId)
        {
            IEnumerable<ISite> siteList;

            siteList = UserIsPortalAdmin || CurrentSession.UserId == userId ?
                _siteService.GetAll(Environment, true).ToList() :
                _siteService.GetByUserAndFunctionKeyIncludeGroup(
                    CurrentSession.UserId,
                    FunctionKeys.AssignSiteToUser,
                    Environment).ToList();

            var siteLevelList = _siteService.GetSiteLevelByUser(userId, Environment);
            var userSiteList = siteList.Where(s => siteLevelList.Any(sl => sl.Site.Id == s.Id)).ToList();

            userSiteList = userSiteList.Union(GetUserUnassignedSites(userId)).ToList();
            GetUserHasFunctionPermissionSites(userSiteList);

            return Json(userSiteList, JsonRequestBehavior.AllowGet);
        }

        private void GetUserHasFunctionPermissionSites(List<ISite> userSiteList)
        {
            var loginUserHasFuntionPermissionSites = _siteService.GetByUserAndFunctionKeyIncludeGroup(
                CurrentSession.UserId,
                FunctionKeys.AssignFunctionToUser,
                Environment);

            foreach (var site in loginUserHasFuntionPermissionSites)
            {
                if (!userSiteList.Any(s => s.Id == site.Id))
                {
                    userSiteList.Add(site);
                }
            }
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.AssignFunctionToUser, Policy = Constant.Policy.Read)]
        public JsonResult GetFunctions(int siteId, int userId)
        {
            var user = _userService.GetById(userId);
            var groupFunctionPolicies = new List<FunctionPolicy>();

            if (!ReferenceEquals(user.Group, null))
            {
                groupFunctionPolicies = _functionService.GetByGroupAndSite(user.Group.Id, siteId, Environment).ToList();
            }

            var functions = _functionService.GetBySiteInHierarchy(siteId, Environment, true).ToList();
            var functionPolicies = _functionService.GetByUserAndSite(userId, siteId, Environment).ToList();
            var loginUserFunctionPolicies = _functionService.GetByUserAndSiteIncludeGroup(CurrentSession.UserId, siteId, Environment).ToList();
            var loginUserAssignFunctionPermission = _userService.GetUserSiteFunctionKeyPolicy(
                    CurrentSession.UserId,
                    siteId,
                    FunctionKeys.AssignFunctionToUser);

            var loginUserAssignSitePermission = _userService.GetUserSiteFunctionKeyPolicy(
                    CurrentSession.UserId,
                    siteId,
                    FunctionKeys.AssignSiteToUser);

            return Json(
                new
                {
                    functions,
                    functionPolicies,
                    groupFunctionPolicies,
                    loginUserFunctionPolicies,
                    loginUserAssignFunctionPermission,
                    loginUserAssignSitePermission
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
        public ActionResult GetUserSiteLevels(int userId)
        {
            IEnumerable<SiteLevel> listSiteLive = _siteService.GetSiteLevelByUser(userId, Environment);

            return Json(listSiteLive, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKeys.AssignSiteToUser, Policy = Constant.Policy.Read)]
        public ActionResult GetLoginUserHasAssignSiteHostPermissionSites()
        {
            int assignSitePolicy = _userService.GetUserSiteFunctionKeyPolicy(
                   CurrentSession.UserId,
                   SiteId,
                   FunctionKeys.AssignSiteToUser);

            var editableSites = new List<ISite>();

            if (assignSitePolicy >= (int)Constant.Policy.Host)
            {
                editableSites = _siteService.GetByUserAndFunctionKeyIncludeGroup(
                        CurrentSession.UserId,
                        FunctionKeys.AssignSiteToUser,
                        Environment,
                        policy: Constant.Policy.Host).ToList();
            }

            return Json(editableSites, JsonRequestBehavior.AllowGet);
        }

        #endregion Assign Site/Function To User

        #region Ajax

        /// <summary>
        /// View user list.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
        public ActionResult UserList(int groupId, int siteId = 0)
        {
            if (Request.IsAjaxRequest())
            {
                var userList = _userService.GetByGroupAndSite(groupId, siteId);

                return View(userList);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Get user site.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
        public ActionResult GetUserSite(int userId)
        {
            if (Request.IsAjaxRequest())
            {
                IEnumerable<SiteLevel> listSiteLive = _siteService.GetSiteLevelByUser(userId, Environment);

                return Json(listSiteLive);
            }

            return Json(null);
        }

        /// <summary>
        /// Get user function policy.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult GetUserFunctionPolicy(int userId)
        {
            if (Request.IsAjaxRequest())
            {
                IEnumerable<FunctionPolicy> listFunctionPolicy = _functionService.GetByUser(userId, Environment);

                return Json(listFunctionPolicy);
            }

            return Json(null);
        }

        /// <summary>
        /// Get group function policy.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult GetGroupFunctionPolicy(int groupId)
        {
            if (Request.IsAjaxRequest() && groupId != 0)
            {
                IEnumerable<FunctionPolicy> listFunctionPolicy = _functionService.GetByGroup(groupId, Environment);

                return Json(listFunctionPolicy);
            }

            return Json(null);
        }

        /// <summary>
        /// Add user to group.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="groupId">The group ID.</param>
        /// <returns>Result string.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Host)]
        public string AddUserToGroup(int userId, int groupId)
        {
            if (Request.IsAjaxRequest() && userId > 0 && groupId > 0)
            {
                var criteria = new InsertUserCriteria
                {
                    UserId = userId,
                    GroupId = groupId,
                    CreatedBy = CurrentSession.UserId,
                    CreatedDate = DateTime.Now
                };

                IUser user = _userService.GetById(userId);

                if (user == null)
                {
                    ICommandResult result = _userService.InsertOrUpdate(criteria);

                    if (result != null && result.IsSuccessful)
                    {
                        return Constant.Ok;
                    }
                }
            }

            return Constant.Error;
        }

        /// <summary>
        /// Get user by group.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [NapAuth(FunctionKey = FunctionKeys.ManageUser, Policy = Constant.Policy.Read)]
        public ActionResult GetUserByGroup(int groupId)
        {
            if (Request.IsAjaxRequest())
            {
                IEnumerable<IUser> listUser = _userService.GetByGroupAndSite(groupId);

                return Json(listUser);
            }

            return Json(null);
        }

        #endregion Ajax

        #region Private method

#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high

        private bool CheckPermissionToUpdateUser(int userId, int siteId, int level)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            if (level == (int)Constant.Level.PortalAdmin)
            {
                return false;
            }

            // if user is not edit himself
            if (CurrentSession.UserId != userId)
            {
                if (UserIsPortalAdmin)
                {
                    return true;
                }

                // check assign site permission in current site
                if (!HasRight(FunctionKeys.AssignSiteToUser, Constant.Policy.Host))
                {
                    return false;
                }

                // check assign site permission in selected site
                var editableSiteList = _siteService.GetByUserAndFunctionKeyIncludeGroup(
                    CurrentSession.UserId,
                    FunctionKeys.AssignSiteToUser,
                    Environment,
                    policy: Constant.Policy.Host).ToList();

                if (!editableSiteList.Any(s => s.Id == siteId))
                {
                    return false;
                }

                // check if current user is a user of selected site
                List<SiteLevel> currentUserSiteLevelList = _siteService.GetSiteLevelByUser(CurrentSession.UserId, Environment).ToList();

                if (!currentUserSiteLevelList.Any(s => s.Site.Id == siteId))
                {
                    return false;
                }

                int currentUserSiteLevel = currentUserSiteLevelList.FirstOrDefault(s => s.Site.Id == siteId).Level;

                // check if selected user is a user of selected site
                List<SiteLevel> siteLevelList = _siteService.GetSiteLevelByUser(userId, Environment).ToList();

                if (!siteLevelList.Any(s => s.Site.Id == siteId))
                {
                    return false;
                }

                int selectedUserSiteLevel = siteLevelList.FirstOrDefault(s => s.Site.Id == siteId).Level;

                // check if current user level, selected user current level and new level are ok
                return currentUserSiteLevel == (int)Constant.Level.User ?
                    selectedUserSiteLevel == (int)Constant.Level.User && level == (int)Constant.Level.User :
                    currentUserSiteLevel > selectedUserSiteLevel && currentUserSiteLevel > level;
            }
            else
            {
                if (SiteId != siteId && UserIsPortalAdmin)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Generates the token for login form.
        /// </summary>
        /// <returns>Generated token.</returns>
        private string GenerateToken()
        {
            string token = Guid.NewGuid().ToString("N");

            FormToken = token;
            AddTokenToCookie(token);

            return token;
        }

        /// <summary>
        /// Adds the login token to cookie.
        /// </summary>
        /// <param name="token">The changing password token.</param>
        private void AddTokenToCookie(string token)
        {
            var responseCookie = new HttpCookie(FormTokenKey)
            {
                HttpOnly = true,
                Value = token
            };

            // If we are using SSL, the cookie should be set to secure to
            // prevent it from being sent over HTTP connections
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }

            // Add the cookie to the response
            Response.Cookies.Set(responseCookie);
        }

        /// <summary>
        /// Checks if the token is valid or not.
        /// </summary>
        /// <param name="token">The changing password token.</param>
        /// <returns>Boolean value indicates if the login token is valid or not.</returns>
        private bool CheckValidToken(string token)
        {
            return !string.IsNullOrWhiteSpace(token)
                && FormToken == token
                && Request.Cookies[FormTokenKey] != null
                && token == Request.Cookies[FormTokenKey].Value;
        }

        /// <summary>
        /// Gets the hash value of input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Hash value of input.</returns>
        private string GetHash(string input)
        {
            var encrypted = string.Empty;

            if (!string.IsNullOrWhiteSpace(input))
            {
                var hashData = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLowerInvariant();
            }

            return encrypted;
        }

        private List<ISite> GetUserUnassignedSites(int userId)
        {
            var loginUserSiteLevelList = _siteService.GetSiteLevelByUser(CurrentSession.UserId, Environment).ToList();
            var loginUserHasAssignSiteHostRightSites = _siteService.GetByUserAndFunctionKeyIncludeGroup(
                        CurrentSession.UserId,
                        FunctionKeys.AssignSiteToUser,
                        Environment,
                        policy: Constant.Policy.Host).ToList();

            var availableSites = _siteService.GetAvailableSitesForUser(userId, Environment).ToList();
            var unassignedSite = new List<ISite>();

            foreach (var site in availableSites)
            {
                var loginUserSiteLevel = loginUserSiteLevelList.FirstOrDefault(s => s.Site.Id == site.Id);

                if (loginUserSiteLevel != null && loginUserHasAssignSiteHostRightSites != null &&
                    loginUserSiteLevel.Level >= (int)Constant.Level.User &&
                    loginUserHasAssignSiteHostRightSites.Any(s => s.Id == site.Id))
                {
                    unassignedSite.Add(site);
                }
            }

            return unassignedSite;
        }

        #endregion Private method
    }
}