import { loadQueryConfig } from "./configuration/query";
import { loadGraphQLSchema } from "./configuration/schema";
import { gateway } from "./gateway";
import * as path from "path";
import { buildQueryResolvers } from "./resolver";

async function main() {
    const graphqlSchema = loadGraphQLSchema(
        path.join(__dirname, "schema.graphql")
    );
    console.log("Using schema:\n", graphqlSchema);

    const queryConfig = loadQueryConfig(
        path.join(__dirname, "query.config.json")
    );

    const app = gateway({
        typeDefs: graphqlSchema,
        resolvers: buildQueryResolvers(queryConfig),
        port: 8080,
    });

    await app.start();
}

main();
