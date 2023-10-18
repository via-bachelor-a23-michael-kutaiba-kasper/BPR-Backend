import * as path from "path";
import * as fs from "fs";

export type QueryConfiguration = {
    queries: QueryDeclaration[];
};

export type QueryDeclaration = {
    name: string;
    args: QueryArgument[];
    resolver: ResolverDeclaration;
};

export type QueryArgument = {
    name: string;
    type: string;
    for: "url" | "body";
};

export type ResolverDeclaration = {
    host: string;
    endpoint: string;
    method: "GET" | "POST" | "PUT" | "DELETE" | "PATCH";
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
