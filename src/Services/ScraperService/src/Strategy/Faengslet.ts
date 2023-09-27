import { Event } from "../Models/Event";
import { IScraperStrategy } from "./IScraperStrategy";
import { chromium } from "playwright";

export class FaengsletScraperStrategy implements IScraperStrategy {
    private eventsUrl = "https://www.faengslet.dk/det-sker";

    constructor() {}

    async scrape(): Promise<Event[]> {
        const browser = await chromium.launch();
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
            console.log(url);
            await page.goto(url);

            const titleElement = page.locator("h1.h1-small");
            const title = (await titleElement.textContent()) ?? "";
            console.log(title);

            const timeElement = page.locator("div.event-line").first();
            let timeString = await timeElement.textContent();
            timeString = timeString ? timeString.trim() : "";
            console.log(timeString);

            let price = "";

            if ((await page.locator("div.event-price").count()) > 0) {
                const priceElement = page.locator("div.event-line").nth(1);
                price = (await priceElement.textContent()) ?? "";
            }

            console.log(price);

            const descriptionElement = page.locator("div.rte > p").first();
            const description =
                (await descriptionElement.textContent()) ?? undefined;

            const imageElement = page.locator("picture > img").first();
            const imageUrl = (await imageElement.getAttribute("src")) ?? "";

            const event = {
                title,
                price,
                host: "Faengslet",
                description,
                url: page.url(),
                location: {
                    country: "Denmark",
                    city: "Horsens",
                    postalCode: "8700",
                    streetName: "Fussingsvej",
                    streetNumber: "8",
                },
                images: [imageUrl],
            };
            console.log(event);
            scrapedEvents.push(event);
        }
        await browser.close();

        return scrapedEvents;
    }
}
