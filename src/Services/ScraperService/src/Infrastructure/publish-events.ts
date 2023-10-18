import { Event } from "src/Models/Event";
import { quickstart } from "./PubsubTopicFactory";

export async function publishEvents(
    events: Event[],
    gcpProject: string,
    topicName: string
): Promise<any> {
    const topic = await quickstart(gcpProject, topicName);
    await topic.publishMessage({ json: events });
    console.log("Message published");
}
