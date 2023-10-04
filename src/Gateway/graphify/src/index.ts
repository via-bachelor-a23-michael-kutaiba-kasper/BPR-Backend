import { loadGraphQLSchema } from "./configuration/load-schema";
import { gateway } from "./gateway";
import * as path from "path";

async function main() {
    const graphqlSchema = loadGraphQLSchema(
        path.join(__dirname, "schema.graphql")
    );
    console.log("Using schema:\n", graphqlSchema);
    const resolvers = {
        Query: {
            events: () => [],
        },
    };

    const app = gateway({
        typeDefs: graphqlSchema,
        resolvers: resolvers,
        port: 8080,
    });

    await app.start();
}

main();
