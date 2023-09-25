import { ScraperConfig } from "./ScraperConfig";
import * as path from "path";
import * as fs from "fs";

export function loadConfig(configPath?: string): ScraperConfig {
    configPath = configPath
        ? path.resolve(configPath)
        : path.resolve(__dirname, "scraper.config.json");
    try {
        if (!fs.existsSync(configPath)) {
            throw new Error(
                `No scraper configuration located at: ${configPath}`
            );
        }

        const fileContents = fs.readFileSync(configPath, { encoding: "utf-8" });
        if (!fileContents) {
            throw new Error(`Config located at ${configPath} is empty`);
        }

        return JSON.parse(fileContents) as ScraperConfig;
    } catch (e) {
        console.error("Failed to load configuration", e);
        throw e;
    }
}
