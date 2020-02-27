requireAll((<any>require).context('./specs', true, /spec.ts$/));

function requireAll(r: any): any {
    r.keys().forEach(r);
}