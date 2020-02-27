import Vue from 'vue';
import Component from 'vue-class-component';
import { Function } from '../../model/function';

@Component({
    template: require('./function-name.html'),
    props: {
        currentFunction: Object,
        functionList: Array
    },
})

export class FunctionNameComponent extends Vue {
    get FunctionLevel(): number {
        return this.generateFunctionLevel(this.$props.functionList, this.$props.currentFunction, 0);
    }

    private generateFunctionLevel(functionList: Array<Function>, currentFunction: Function, level: number): number {
        if (currentFunction.Parent === null) {
            return level;
        }

        const parent = functionList.find(f => f.Id === currentFunction.Parent.Id);

        return this.generateFunctionLevel(functionList, parent, level + 1);
    }
}