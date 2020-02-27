import Vue from 'vue';
import _ from 'lodash';

export default class ComponentTest {
    private vm: Vue;

    constructor(private component: Vue) {
    }

    public mount(createOptions?: any): void {
        if (createOptions) {
            _.merge(this.component.$options, createOptions);
        };

        this.vm = new Vue(this.component.$options).$mount();
    }

    public find(query: string): HTMLInputElement {
        return <HTMLInputElement>this.vm.$el.querySelector(query);
    }

    public get current(): Vue {
        return this.vm
    }
}