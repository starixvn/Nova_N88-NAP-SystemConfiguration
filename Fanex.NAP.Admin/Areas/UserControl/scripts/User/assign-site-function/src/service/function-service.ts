import { Function, FunctionPolicy } from '../model/function';
import axios, { AxiosResponse } from 'axios';
import MessageBox from '../util/message-box';

export default new class FunctionService {
    protected axios;

    constructor() {
        this.axios = axios;
    }

    GetFunctions(rootRoute: string, userId: string, siteId: number,
        callback: (response: {
            functions: Array<Function>,
            functionPolicies: Array<FunctionPolicy>,
            groupFunctionPolicies: Array<FunctionPolicy>,
            loginUserFunctionPolicies: Array<FunctionPolicy>,
            loginUserAssignFunctionPermission: number,
            loginUserAssignSitePermission: number
        }) => void) {
        this.axios.get(rootRoute + 'User/GetFunctions?userId=' + userId + '&siteId=' + siteId).then((response: AxiosResponse) => {
            callback(response.data);
        }, (error) => {
            MessageBox.ShowError(error);
        });
    }

    UpdateFunctions(rootRoute: string, payload: any, callback: (result: any) => void) {
        axios.post(rootRoute + 'User/AssignFunction', payload).then((response: AxiosResponse) => {
            callback(response.data);
        }, (error) => {
            MessageBox.ShowError(error);
        });
    }
};