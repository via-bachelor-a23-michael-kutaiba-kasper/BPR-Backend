import { Event } from "../../Models/Event";
export interface IScraperStrategy {
    scrape(): Promise<Event[]>;
}
