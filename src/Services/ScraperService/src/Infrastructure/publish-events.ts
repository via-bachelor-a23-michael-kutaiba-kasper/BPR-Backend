import { Event } from "src/Models/Event";
import { quickstart } from "./PubsubTopicFactory";

export async function publishEvents(events: Event[]): Promise<any> {
    const topic = await quickstart("pubsubtest");
    await topic.publishMessage({ json: events });
    console.log("Message published");
}
