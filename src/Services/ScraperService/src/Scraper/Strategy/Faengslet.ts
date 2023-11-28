import { Event } from "../../Models/Event";
import { IScraperStrategy } from "./IScraperStrategy";
import { chromium as pwChromium } from "playwright";
import chromium from "@sparticuz/chromium";
import { ConfigOptions } from "src/Configuration/ScraperConfig";

export class FaengsletScraperStrategy implements IScraperStrategy {
    private eventsUrl = "https://www.faengslet.dk/det-sker";

    constructor() {}

    async scrape(config: ConfigOptions): Promise<Event[]> {
        const browser = await pwChromium.launch({
            args: chromium.args,
            executablePath: await chromium.executablePath(),
        });
        const page = await browser.newPage();
        await page.goto(this.eventsUrl);

        const eventGridItems = page.locator("a.grid-item");
        const scrapedEvents: Event[] = [];

        let urls = (await eventGridItems.all())
            .map(async (locator) => await locator.getAttribute("href"))
            .map(
                async (relativeUrl) =>
                    `https://www.faengslet.dk${await relativeUrl}`
            );

        const urls2 = await Promise.all(urls);

        for (let url of urls2) {
            await page.goto(url);

            const titleElement = page.locator("h1.h1-small");
            const title = (await titleElement.textContent()) ?? "";

            const timeElement = page.locator("div.event-line").first();
            let timeString = await timeElement.textContent();
            timeString = timeString ? timeString.trim() : "";

            let price = "";

            if ((await page.locator("div.event-price").count()) > 0) {
                const priceElement = page.locator("div.event-line").nth(1);
                price = (await priceElement.textContent()) ?? "";
            }

            const descriptionElement = page.locator("div.rte > p").first();
            const description =
                (await descriptionElement.textContent()) ?? undefined;

            const imageElement = page.locator("picture > img").first();
            const imageUrl = (await imageElement.getAttribute("src")) ?? "";

            const event: Event = {
                title,
                price,
                host: "Faengslet",
                description,
                url: page.url(),
                hostId: config.hostId,
                location: "Fussingvej 8, 8700 Horsens, Denmark",
                city: "Horsens",
                lat: 55.87406,
                lng: 9.8363,
                images: [imageUrl],
                adultsOnly: false,
                isPaid: price != null || price != undefined,
                isPrivate: false,
                keywords: [],
                attendees: [],
            };
            scrapedEvents.push(event);
        }
        await browser.close();

        return scrapedEvents;
    }
}
