import { ConfigOptions } from "src/Configuration/ScraperConfig";
import { Event } from "../../Models/Event";
export interface IScraperStrategy {
    scrape(config: ConfigOptions): Promise<Event[]>;
}
