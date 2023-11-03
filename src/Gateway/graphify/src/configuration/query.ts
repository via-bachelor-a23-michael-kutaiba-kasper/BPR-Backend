import * as path from "path";
import * as fs from "fs";

export type QueryConfiguration = {
    queries: QueryDeclaration[];
    mutations: QueryDeclaration[];
};

export type QueryDeclaration = {
    name: string;
    args: QueryArgument[];
    resolver: ResolverDeclaration;
};

export type QueryArgument = {
    name: string;
    type: string;
    for: "urlPath" | "urlQueryParam" | "body";
};

export type ResolverDeclaration = {
    host: string;
    port: number;
    endpoint: string;
    method: "GET" | "POST" | "PUT" | "DELETE" | "PATCH";
    headers?: Map<string, string>;
};

const QUERY_CONFIG_FILE = "query.config.json";

export function loadQueryConfig(configPath: string): QueryConfiguration {
    configPath = path.resolve(configPath);
    if (!fs.existsSync(configPath)) {
        throw new Error(`path ${configPath} does not exist`);
    }

    if (fs.lstatSync(configPath).isDirectory()) {
        configPath = path.join(configPath, QUERY_CONFIG_FILE);
    }

    const configContents = fs.readFileSync(path.resolve(configPath), "utf-8");
    const config = JSON.parse(configContents);

    return config;
}
