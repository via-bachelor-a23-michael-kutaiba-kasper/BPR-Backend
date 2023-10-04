import { ApolloServer } from "@apollo/server";
import { startStandaloneServer } from "@apollo/server/standalone";

export type GatewayConfig = {
    typeDefs: string;
    resolvers: {
        Query: any;
    };
    port: number | undefined | null;
};

export type Gateway = {
    start: () => Promise<void>;
};

export function gateway(config: GatewayConfig): Gateway {
    const server = new ApolloServer({
        typeDefs: config.typeDefs,
        resolvers: config.resolvers,
    });

    const start = async () => {
        const { url } = await startStandaloneServer(server, {
            listen: { port: config.port ?? 4242, host: "0.0.0.0" },
        });

        console.log(`🚀  Server ready at: ${url}`);
    };

    return { start };
}
