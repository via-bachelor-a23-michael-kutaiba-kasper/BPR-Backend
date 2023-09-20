import { Event } from "../Models/Event";
import { IScraperStrategy } from "./IScraperStrategy";

export class FaengsletScraperStrategy implements IScraperStrategy {
  async scrape(): Promise<Event> {
    const { firefox } = require("playwright"); // Or 'chromium' or 'webkit'.

    const browser = await firefox.launch();
    const page = await browser.newPage();
    await page.goto("https://example.com");
    await browser.close();
    throw Error;
  }
}
