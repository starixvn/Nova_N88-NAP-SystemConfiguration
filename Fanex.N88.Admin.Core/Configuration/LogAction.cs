namespace Fanex.N88.Admin.Core.Configuration
{
    public enum LogAction
    {
        N88_Cust_Insert = 100,

        N88_Cust_Update = 200,

        N88_Cust_UpdatePassword = 3,

        N88_Function_AssignFunctionToGroups = 300,

        N88_Function_AssignFunctionToUsers = 301,

        N88_Function_CopyPermission = 406,

        N88_Function_Duplicate = 212,

        N88_Function_Insert = 101,

        N88_Function_Update = 201,

        N88_Function_UpdateStatus = 202,

        N88_Group_AssignFunction = 302,

        N88_Group_Insert = 102,

        N88_Group_Update = 203,

        N88_Group_UpdateStatus = 204,

        N88_MenuItem_Insert = 104,

        N88_MenuItem_Update = 205,

        N88_MenuItem_UpdateStatus = 206,

        N88_Site_Insert = 103,

        N88_Site_InsertSiteURL = 105,

        N88_Site_InsertWithSiteURL = 106,

        N88_Site_Update = 207,

        N88_Site_UpdateDefaultSiteURL = 208,

        N88_Site_UpdateSiteURL = 209,

        N88_Site_UpdateStatus = 211,

        N88_User_AssignFunction = 304,

        N88_User_AssignNewSite = 403,

        N88_User_AssignSite = 400,

        N88_User_ChangeUserSiteStatus = 210,

        N88_User_CheckIn = 27,

        N88_User_Insert = 107,

        N88_User_SSOCheckIn = 28,

        N88_User_UpdateSite = 404,

        Task_N88_RevokeUserPermission = 405
    }
}