export type ScraperConfig = Map<string, ConfigOptions>;
export type ConfigOptions = {
    useHeadless: boolean;
    browser: "chrome";
    hostId: string;
};
