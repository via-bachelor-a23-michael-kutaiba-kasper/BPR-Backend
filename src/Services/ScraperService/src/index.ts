// import { FaengsletScraperStrategy } from "./Strategy/Faengslet";
// console.log("Started scraping");
// const strategy = new FaengsletScraperStrategy();
// console.log("Scraper created");
// strategy.scrape().then((d) => console.log(d));

import { loadConfig } from "./Configuration/load-config";

const config = loadConfig();
console.log(config);
