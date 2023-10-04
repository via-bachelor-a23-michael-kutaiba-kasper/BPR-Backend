import { PubSub, Topic, Subscription } from "@google-cloud/pubsub";

// re,ember renaming the project in the compose file
export async function quickstart(
  projectId = "pubsubtest", // Your Google Cloud Platform project ID
  topicNameOrId = "test" // Name for the new topic to create
): Promise<Topic> {
  // Instantiates a client
  const pubsub = new PubSub({ projectId });

  return new Promise((resolve, reject) => {
    pubsub.topic(topicNameOrId).get({
      autoCreate: true
    }, (err, topic, _) => {
      if (err) {
        reject(err);
      }
      if (!topic) {
        reject(err);
      } else {
        resolve(topic);
      }
    });
  });
}

export async function getSub(
  subName: string,
  topic: Topic
): Promise<Subscription> {
  return new Promise((resolve, reject) => {
    topic.subscription(subName).get({
      autoCreate: true
    },(err, sub, _) => {
      if (err) {
        reject(err);
      }
      if (!sub) {
        reject(err);
      } else {
        resolve(sub);
      }
    });
  });
}
