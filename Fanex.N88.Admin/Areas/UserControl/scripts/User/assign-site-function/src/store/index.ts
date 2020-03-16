import Vue from 'vue';
import Vuex from 'vuex';
import SiteModule from './modules/site';

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        SiteModule
    },
});