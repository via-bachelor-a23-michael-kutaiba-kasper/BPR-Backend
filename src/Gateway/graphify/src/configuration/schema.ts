import * as fs from "fs";
import * as path from "path";

const SCHEMA_FILE = "schema.graphql";

export function loadGraphQLSchema(schemaPath: string): string {
    schemaPath = path.resolve(schemaPath);
    if (!fs.existsSync(schemaPath)) {
        throw new Error(`path ${schemaPath} does not exist`);
    }

    if (fs.lstatSync(schemaPath).isDirectory()) {
        schemaPath = path.join(schemaPath, SCHEMA_FILE);
    }

    const schemaContents = fs.readFileSync(path.resolve(schemaPath), "utf-8");
    return schemaContents;
}
