import { ScraperConfig } from "src/Configuration/ScraperConfig";
import { Event } from "src/Models/Event";
import { IStrategyFactory, StrategyFactory } from "./Strategy/StrategyFactory";

export interface IScraper {
    scrapeAll(): Promise<Event[]>;
    scrape(strategyName: string): Promise<Event[]>;
}

export class Scraper implements IScraper {
    private _strategyFactory: IStrategyFactory = new StrategyFactory();

    constructor(private _config: ScraperConfig) {}

    scrape(strategyName: string): Promise<Event[]> {
        return this._strategyFactory.create(strategyName).scrape();
    }

    async scrapeAll(): Promise<Event[]> {
        let events = [] as Event[];

        for (let strategyName of this._config.keys()) {
            const strategy = this._strategyFactory.create(strategyName);
            events = [...events, ...(await strategy.scrape())];
        }

        return events;
    }
}
