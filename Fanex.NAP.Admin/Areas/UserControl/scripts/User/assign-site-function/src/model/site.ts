import { Function, FunctionPolicy } from './function';

export interface Site {
    Description?: string;
    Enviroment?: string;
    IsActive?: boolean;
    NumOfUser?: number;
    SecretAccessKey?: string;
    SiteKey?: string;
    Id?: number;
    Name?: string;
    ShowFunctionList?: boolean;
    HasAllFunctionUserReadPolicy?: boolean;
    HasAllFunctionUserHostPolicy?: boolean;
    HasAllFunctionGroupReadPolicy?: boolean;
    HasAllFunctionGroupHostPolicy?: boolean;
    DisableCheckBoxAllFunctionReadPolicy?: boolean;
    DisableCheckBoxAllFunctionHostPolicy?: boolean;
    HideCheckBoxAllFunctionPolicy?: boolean;
    Functions?: Array<Function>;
    ViewedUserFunctionPolicies?: Array<FunctionPolicy>;
    ViewedUserGroupFunctionPolicies?: Array<FunctionPolicy>;
    LoginUserFunctionPolicies?: Array<FunctionPolicy>;
    LoginUserAssignFunctionPermission?: number;
    LoginUserAssignSitePermission?: number;
    SiteLevel?: SiteLevel;
    EnableEdit?: boolean;
    EnableAssign?: boolean;
    HasFunctionsChange?: boolean;
}

export interface SiteLevel {
    Level?: number;
    IsActive?: boolean;
    IsNeverExpired?: boolean;
    ExpiredDate?: string;
    Site?: Site;
}

export enum SiteLevelValue {
    User = 0,
    SiteAdmin = 50,
    PortalAdmin = 100
}