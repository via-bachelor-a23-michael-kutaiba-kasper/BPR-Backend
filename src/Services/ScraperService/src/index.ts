import { FaengsletScraperStrategy } from "./Strategy/Faengslet";

import { quickstart } from "./Infrastructure/PubsubTopicFactory";

async function main() {
    // Creates a new topic
    const topic = await quickstart("pubsubtest");
    console.log(await topic.exists());

    console.log("Started scraping");
    const strategy = new FaengsletScraperStrategy();
    console.log("Scraper created");
    const event = await strategy.scrape();

    console.log(topic.name);

    // Send a message to the topic
    await topic.publishMessage({ json: event });

    console.log("Message published");
}
main();
