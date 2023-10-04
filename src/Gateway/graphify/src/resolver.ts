import { QueryConfiguration, QueryDeclaration } from "./configuration/query";

export function buildQueryResolvers(config: QueryConfiguration): {
    Query: any;
} {
    const resolvers = {};
    config.queries.forEach((query) => buildResolver(resolvers, query));
    return { Query: resolvers };
}

function buildResolver(resolvers: any, config: QueryDeclaration) {
    resolvers[config.name] = async (
        parent: any,
        args: any,
        contextValue: any,
        info: any
    ) => {
        let bodyArg = config.args.find((arg) => arg.for === "body");
        let body = bodyArg ? JSON.stringify(args[bodyArg.name]) : undefined;

        const requestOptions: RequestInit = {
            method: config.resolver.method,
            body,
        };

        const url = expandUrlParams(args, config);
        const res = await fetch(config.resolver.url, requestOptions);
        return res.json();
    };
}

function expandUrlParams(args: any, config: QueryDeclaration): string {
    const argMap = new Map<string, any>();
    const urlArgs = config.args.filter((arg) => arg.for === "url");

    if (urlArgs.length === 0) {
        return config.resolver.url;
    }

    urlArgs.forEach((arg) => argMap.set(arg.name, args[arg.name]));

    const urlParts = config.resolver.url.split("/");
    urlParts.forEach((part, idx) => {
        if (part.startsWith("{") && part.endsWith("}")) {
            const argName = part.slice(1, part.length - 1);
            urlParts[idx] = argMap.get(argName);
        }
    });

    return urlParts.join("/");
}
