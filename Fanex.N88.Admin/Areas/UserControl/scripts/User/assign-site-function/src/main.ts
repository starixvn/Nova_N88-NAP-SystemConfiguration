import * as Vue from 'vue';
import { SiteListComponent } from './components/site-list';
import store from './store';
import './style/main.css';

new Vue({
    el: '#app-main',
    store: store,
    components: {
        'site-list': SiteListComponent
    }
});