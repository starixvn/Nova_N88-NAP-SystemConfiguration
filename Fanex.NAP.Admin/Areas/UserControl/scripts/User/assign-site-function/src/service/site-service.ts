import { Site, SiteLevel } from '../model/site';
import axios, { AxiosResponse } from 'axios';
import MessageBox from '../util/message-box';

export default new class SiteService {
    protected axios;

    constructor() {
        this.axios = axios;
    }

    GetSites(rootRoute: string, userId: string, callback: (sites: Array<Site>) => void) {
        this.axios.get(rootRoute + 'User/GetAllUserSites?userId=' + userId).then((response: AxiosResponse) => {
            callback(response.data);
        }, (error) => {
            MessageBox.ShowError(error);
        });
    }

    GetUserSitesLevels(rootRoute: string, userId: string, callback: (siteLevels: Array<SiteLevel>) => void) {
        this.axios.get(rootRoute + 'User/GetUserSiteLevels?userId=' + userId).then((response: AxiosResponse) => {
            callback(response.data);
        }, (error) => {
            MessageBox.ShowError(error);
        });
    }

    GetLoginUserHasAssignSiteHostPermissionSites(rootRoute: string, callback: (site: Array<Site>) => void) {
        this.axios.get(rootRoute + 'User/GetLoginUserHasAssignSiteHostPermissionSites').then((response: AxiosResponse) => {
            callback(response.data);
        }, (error) => {
            MessageBox.ShowError(error);
        });
    }

    UpdateSiteToUser(rootRoute: string, userId: string, site: Site, callback: (result: string) => void) {
        this.axios.post(rootRoute + 'User/UpdateSiteToUser?userId=' + userId +
            '&siteId=' + site.Id +
            '&level=' + site.SiteLevel.Level +
            '&isActive=' + site.SiteLevel.IsActive +
            '&expiredDate=' + new Date(site.SiteLevel.ExpiredDate).toDateString() +
            '&isNeverExpired=' + site.SiteLevel.IsNeverExpired).then((response: AxiosResponse) => {
                callback(response.data);
            }, (error) => {
                MessageBox.ShowError(error);
            });
    }

    AssignSiteToUser(rootRoute: string, userId: string, site: Site, callback: (result: string) => void) {
        this.axios.post(rootRoute + 'User/AssignSiteToUser?userId=' + userId +
            '&siteId=' + site.Id +
            '&level=' + site.SiteLevel.Level +
            '&isActive=' + site.SiteLevel.IsActive).then((response: AxiosResponse) => {
                callback(response.data);
            }, (error) => {
                MessageBox.ShowError(error);
            });
    }
};