import { QueryConfiguration, QueryDeclaration } from "./configuration/query";

export function buildQueryResolvers(config: QueryConfiguration): {
    Query: any;
    Mutation: any;
} {
    const queryResolvers = {};
    config.queries.forEach((query) => buildResolver(queryResolvers, query));
    const mutationResolvers = {};
    config.mutations.forEach((mutation) =>
        buildResolver(mutationResolvers, mutation)
    );
    return { Query: queryResolvers, Mutation: mutationResolvers };
}

function buildResolver(resolvers: any, config: QueryDeclaration) {
    resolvers[config.name] = async (
        parent: any,
        args: any,
        contextValue: any,
        info: any
    ) => {
        let bodyObj = {} as any;
        for (let passedArg of Object.keys(args)) {
            const configuredArg = config.args.find(
                (arg) => arg.name === passedArg
            );
            if (!configuredArg) {
                continue;
            }

            if (configuredArg.for != "body") {
                continue;
            }

            bodyObj[passedArg] = args[passedArg];
        }
        console.log(bodyObj);
        let hasBodyArgs = config.args.some((arg) => arg.for === "body");
        let body = hasBodyArgs ? JSON.stringify(bodyObj) : undefined;

        const requestOptions: RequestInit = {
            method: config.resolver.method,
            headers: config.resolver.headers as HeadersInit | undefined,
        };

        if (hasBodyArgs) {
            requestOptions.body = body;
        }

        const url = expandUrlParams(args, config);
        console.log(`Routing request to: ${url}`);
        console.log(
            `Request Options: ${JSON.stringify(requestOptions, null, 4)}`
        );
        try {
            const res = await fetch(url, requestOptions);
            try {
                const resBody = await res.json();
                console.log(
                    `Received response: ${JSON.stringify(
                        res.status,
                        undefined,
                        4
                    )}`
                );
                console.log(
                    `Response body: ${JSON.stringify(resBody, undefined, 4)}\n`
                );

                return resBody;
            } catch (err) {
                console.error(err);
            }
            return {};
        } catch (err) {
            console.error(err);
            throw err;
        }
    };
}

function formatToQueryParams(
    args: any,
    config: QueryDeclaration
): string | undefined {
    let values: string[] = [];
    const queryParamArgs = config.args.filter(
        (arg) => arg.for === "urlQueryParam"
    );

    if (queryParamArgs.length === 0) {
        return undefined;
    }

    queryParamArgs.forEach((arg) => {
        values.push(`${arg.name}=${args[arg.name]}`);
    });

    return values.join("&");
}

function expandUrlParams(args: any, config: QueryDeclaration): string {
    const argMap = new Map<string, any>();

    let host =
        process.env[`QUERY_${config.name.toUpperCase()}_HOST`] ??
        config.resolver.host;

    const urlArgs = config.args.filter((arg) => arg.for === "urlPath");
    let queryParams = formatToQueryParams(args, config);
    if (urlArgs.length === 0) {
        return queryParams
            ? `${host}${config.resolver.endpoint}?${queryParams}`
            : `${host}${config.resolver.endpoint}?`;
    }
    urlArgs.forEach((arg) => argMap.set(arg.name, args[arg.name]));

    const endpointParts = config.resolver.endpoint.split("/");
    endpointParts.forEach((part, idx) => {
        if (part.startsWith("{") && part.endsWith("}")) {
            const argName = part.slice(1, part.length - 1);
            endpointParts[idx] = argMap.get(argName);
        }
    });

    let endpointExpanded = endpointParts.join("/");

    return queryParams
        ? `${host}:${config.resolver.port}${endpointExpanded}?${queryParams}`
        : `${host}:${config.resolver.port}${endpointExpanded}`;
}
