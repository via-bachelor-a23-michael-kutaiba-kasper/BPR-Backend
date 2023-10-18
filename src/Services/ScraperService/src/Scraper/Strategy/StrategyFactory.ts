import { FaengsletScraperStrategy } from "./Faengslet";
import { IScraperStrategy } from "./IScraperStrategy";

export interface IStrategyFactory {
    create(strategyName: string): IScraperStrategy;
}

export class StrategyFactory implements IStrategyFactory {
    create(strategyName: string): IScraperStrategy {
        switch (strategyName) {
            case "faengslet":
                return new FaengsletScraperStrategy();
            default:
                throw new Error(
                    `Strategy with name ${strategyName} is not supported`
                );
        }
    }
}
