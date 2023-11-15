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
        const strategyConfiguration = this._config.get(strategyName);
        if (!strategyConfiguration) {
            throw new Error(
                `Strategy ${strategyName} has not been configured yet`
            );
        }
        return this._strategyFactory
            .create(strategyName)
            .scrape(strategyConfiguration);
    }

    async scrapeAll(): Promise<Event[]> {
        let events = [] as Event[];

        for (let strategyName of this._config.keys()) {
            const strategyConfiguration = this._config.get(strategyName);
            if (!strategyConfiguration) {
                throw new Error(
                    `Strategy ${strategyName} has not been configured yet`
                );
            }
            const strategy = this._strategyFactory.create(strategyName);
            events = [
                ...events,
                ...(await strategy.scrape(strategyConfiguration)),
            ];
        }

        return events;
    }
}
