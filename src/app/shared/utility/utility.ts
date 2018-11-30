export function clone(input: any): any {
    return JSON.parse(JSON.stringify(input))
}
