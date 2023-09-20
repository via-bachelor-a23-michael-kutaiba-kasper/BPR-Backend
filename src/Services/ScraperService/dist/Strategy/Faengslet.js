"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.FaengsletScraperStrategy = void 0;
const playwright_1 = require("playwright");
class FaengsletScraperStrategy {
    constructor() {
        this.eventsUrl = "https://www.faengslet.dk/det-sker";
    }
    async scrape() {
        var _a, _b, _c, _d;
        const browser = await playwright_1.chromium.launch();
        const page = await browser.newPage();
        await page.goto(this.eventsUrl);
        const eventGridItems = page.locator("a.grid-item");
        const scrapedEvents = [];
        let urls = (await eventGridItems.all())
            .map(async (locator) => await locator.getAttribute("href"))
            .map(async (relativeUrl) => `https://www.faengslet.dk${await relativeUrl}`);
        const urls2 = await Promise.all(urls);
        for (let url of urls2) {
            console.log(url);
            await page.goto(url);
            const titleElement = page.locator("h1.h1-small");
            const title = (_a = (await titleElement.textContent())) !== null && _a !== void 0 ? _a : "";
            console.log(title);
            const timeElement = page.locator("div.event-line").first();
            let timeString = await timeElement.textContent();
            timeString = timeString ? timeString.trim() : "";
            console.log(timeString);
            let price = "";
            if ((await page.locator("div.event-price").count()) > 0) {
                const priceElement = page.locator("div.event-line").nth(1);
                price = (_b = (await priceElement.textContent())) !== null && _b !== void 0 ? _b : "";
            }
            console.log(price);
            const descriptionElement = page.locator("div.rte > p").first();
            const description = (_c = (await descriptionElement.textContent())) !== null && _c !== void 0 ? _c : undefined;
            const imageElement = page.locator("picture > img").first();
            const imageUrl = (_d = (await imageElement.getAttribute("src"))) !== null && _d !== void 0 ? _d : "";
            const event = {
                title,
                price,
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
exports.FaengsletScraperStrategy = FaengsletScraperStrategy;
//# sourceMappingURL=Faengslet.js.map