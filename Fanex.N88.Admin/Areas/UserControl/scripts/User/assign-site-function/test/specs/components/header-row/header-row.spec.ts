import Vue from 'vue';
import Vuex from 'vuex';
import { HeaderRowComponent } from '../../../../src/components/header-row';
import { expect } from 'chai';
import sinon from 'sinon';
import SiteModule from '../../../../src/store/modules/site';
import { SiteState } from '../../../../src/store/modules/site/state';
import * as TYPE from '../../../../src/store/types';
import { Site } from '../../../../src/model/site';
import * as _ from 'lodash';
import ComponentTest from '../../../utils/component-test';

Vue.use(Vuex);

describe('HeaderRowComponent', () => {
    let store: Vuex.Store<SiteState>;
    let componentTest: ComponentTest;
    const mutations = {
        ShowHideSiteFunctions: sinon.stub(),
        UpdateSelectAllUserFunctionForAllSites: sinon.stub(),
    };

    const actions = {
        SetAllFunctionsUserPolicyForAllSites: sinon.stub()
    }

    beforeEach(() => {
        componentTest = new ComponentTest(new HeaderRowComponent());

        store = new Vuex.Store({
            state: SiteModule.state,
            getters: SiteModule.getters,
            mutations: mutations,
            actions: actions
        });
    });

    it('has expired date column when [viewedUserName] has [starix] prefix', () => {
        componentTest.mount({
            store: store, propsData: {
                viewedUserName: 'starix1'
            }
        });

        expect(componentTest.find('.expired-date-header').style.display).to.equal('');
    });

    it('has expired date column when [viewedUserName] has no [starix] prefix', () => {
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('.expired-date-header').style.display).to.equal('none')
    });

    it('has disabled [all-read-function-policies-for-all-sites-checkbox]', () => {
        const sites: Array<Site> = [
            { DisableCheckBoxAllFunctionReadPolicy: true }
        ];
        store.state.All = sites;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('#all-read-function-policies-for-all-sites-checkbox').getAttribute('disabled')).to.be.equal('disabled');
    });

    it('has enabled [all-read-function-policies-for-all-sites-checkbox]', () => {
        const sites: Array<Site> = [
            { DisableCheckBoxAllFunctionReadPolicy: false }
        ];
        store.state.All = sites;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('#all-read-function-policies-for-all-sites-checkbox').getAttribute('disabled')).to.be.null;
    });

    it('has checked [all-read-function-policies-for-all-sites-checkbox]', () => {
        store.state.SelectAllReadFunctionsForAllSites = true;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('#all-read-function-policies-for-all-sites-checkbox').checked)
            .to.be.true;
    });

    it('has unchecked [all-read-function-policies-for-all-sites-checkbox]', () => {
        store.state.SelectAllReadFunctionsForAllSites = false;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('#all-read-function-policies-for-all-sites-checkbox').checked).to.be.false;
    });

    it('has disabled [all-host-function-policies-for-all-sites-checkbox]', () => {
        const sites: Array<Site> = [
            { DisableCheckBoxAllFunctionHostPolicy: true }
        ];
        store.state.All = sites;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('#all-host-function-policies-for-all-sites-checkbox').getAttribute('disabled'))
            .to.be.equal('disabled');
    });

    it('has enabled [all-host-function-policies-for-all-sites-checkbox]', () => {
        const sites: Array<Site> = [
            { DisableCheckBoxAllFunctionHostPolicy: false }
        ];
        store.state.All = sites;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('#all-host-function-policies-for-all-sites-checkbox').getAttribute('disabled'))
            .to.be.null;
    });

    it('has checked [all-host-function-policies-for-all-sites-checkbox]', () => {
        store.state.SelectAllHostFunctionsForAllSites = true;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('#all-host-function-policies-for-all-sites-checkbox').checked).to.be.true;
    });

    it('has unchecked [all-host-function-policies-for-all-sites-checkbox]', () => {
        store.state.SelectAllHostFunctionsForAllSites = false;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });

        expect(componentTest.find('#all-host-function-policies-for-all-sites-checkbox').checked).to.be.false;
    });

    it('calls store commit ShowHideSiteFunctions when click on resize button', () => {
        const sites: Array<Site> = [
            { DisableCheckBoxAllFunctionHostPolicy: false, Id: 1 },
            { DisableCheckBoxAllFunctionHostPolicy: false, Id: 2 },
        ];
        store.state.All = sites;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });
        const showAllSiteFunctions: boolean = componentTest.current.$data['showAllSiteFunctions'];

        componentTest.find('.glyphicon-resize-full').click();

        expect(mutations.ShowHideSiteFunctions.getCall(0).args[1])
            .to.deep.equal({ siteId: 1, isShow: !showAllSiteFunctions });
        expect(mutations.ShowHideSiteFunctions.getCall(1).args[1])
            .to.deep.equal({ siteId: 2, isShow: !showAllSiteFunctions });
        expect(mutations.ShowHideSiteFunctions.callCount)
            .to.equal(sites.length);
        expect(componentTest.current.$data['showAllSiteFunctions'])
            .to.equal(!showAllSiteFunctions);
    });

    it('calls store commit/dispatch when click on [all-read-function-policies-for-all-sites-checkbox]', () => {
        store.state.SelectAllReadFunctionsForAllSites = true;
        componentTest.mount({
            store, propsData: {
                viewedUserName: 'abvce'
            }
        });
        const checkBox = componentTest.find('#all-read-function-policies-for-all-sites-checkbox');

        checkBox.click();

        expect(mutations.UpdateSelectAllUserFunctionForAllSites.callCount).to.equal(1);
        expect(actions.SetAllFunctionsUserPolicyForAllSites.callCount).to.equal(1);
        expect(mutations.UpdateSelectAllUserFunctionForAllSites.
            getCall(0).args[1]).to.deep.equal({ policy: false, isRead: true });
        expect(actions.SetAllFunctionsUserPolicyForAllSites.
            getCall(0).args[1]).to.deep.equal({ rootPolicy: false, isRead: true });
    });
});