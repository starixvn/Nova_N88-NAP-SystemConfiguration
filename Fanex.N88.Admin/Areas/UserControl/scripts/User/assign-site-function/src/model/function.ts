import { Site } from './site';

export interface Function {
    Id?: number;
    Name?: string;
    Parent?: Function;
    SiteId?: number;
    HasUserReadPolicy?: boolean;
    HasUserHostPolicy?: boolean;
    HasGroupReadPolicy?: boolean;
    HasGroupHostPolicy?: boolean;
    Crud?: number;
    DisableRead?: boolean;
    DisableHost?: boolean;
    IsHidden?: boolean;
}

export interface FunctionPolicy {
    Function?: Function;
    Site?: Site;
    Policy?: number;
}

export enum FunctionPolicyValue {
    None = 0,
    Read = 1,
    Host = 2
}