import express, { Express, Request, Response } from "express";
import bodyParser from "body-parser";
import cors from "cors";

import { loadConfig } from "./Configuration/load-config";
import { Scraper } from "./Scraper/Scraper";
import { publishEvents } from "./Infrastructure/publish-events";

export type ScrapeDTO = {
    strategy: string;
};

async function main() {
    const scraperConfig = loadConfig();
    const scraper = new Scraper(scraperConfig);

    const GCP_PROJECT = process.env["GCP_PROJECT"] || "bachelorshenanigans";
    const TOPIC_NAME = process.env["GCP_TOPIC_NAME"] || "events:scraped";

    const app: Express = express();
    app.use(bodyParser.json());
    app.use(cors());

    const port = process.env["PORT"] || 8082;

    app.post("/scrape", async (req: Request, res: Response) => {
        const dto: ScrapeDTO = req.body;
        const events = await scraper.scrape(dto.strategy);
        console.log(events);
        await publishEvents(events, GCP_PROJECT, TOPIC_NAME);
        res.status(204).send({});
    });

    app.listen(port, () => {
        console.log(`🚀 Scraper Service is now listening on ${port}`);
    });
}
main();
