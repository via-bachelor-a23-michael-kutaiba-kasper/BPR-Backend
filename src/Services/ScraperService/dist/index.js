"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const Faengslet_1 = require("./Strategy/Faengslet");
console.log("Started scraping");
const strategy = new Faengslet_1.FaengsletScraperStrategy();
console.log("Scraper created");
strategy.scrape().then((d) => console.log(d));
//# sourceMappingURL=index.js.map