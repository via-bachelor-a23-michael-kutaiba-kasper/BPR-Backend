import express, { Express, Request, Response } from "express";
import cors from "cors";
import bodyParser = require("body-parser");

type Job = {
    url: string;
    method: string;
    body: any;
};

async function executeJob(job: Job, incrementer: () => void) {
    let res;
    console.log(`Sending HTTP request to: ${job.url}`);
    if (job.body) {
        console.log(`Payload`, job.body);
        res = await fetch(job.url, {
            method: job.method,
            body: JSON.stringify(job.body),
            headers: {
                "Content-Type": "application/json",
            },
        });
    } else {
        res = await fetch(job.url, {
            method: job.method,
            headers: {
                "Content-Type": "application/json",
            },
        });
    }
    const status = res.status;
    console.log(`Job executed with status: ${status}`);
    incrementer();
}

async function executeJobs(jobs: Job[]): Promise<number> {
    console.log(jobs);
    let exectuedJobs = 0;
    await Promise.all(jobs.map((job) => executeJob(job, () => exectuedJobs++)));

    return exectuedJobs;
}

const app: Express = express();
app.use(cors());
app.use(bodyParser.json());

app.post("/trigger", async (req: Request, res: Response) => {
    const jobs: Job[] = req.body;
    const executedJobs = await executeJobs(jobs);
    console.log(`Executed ${executedJobs} jobs`);
    res.status(204).send({});
});

const port = process.env["PORT"] || 8088;
app.listen(port, () => {
    console.log(`🚀 HTTP Jobs is now listening on ${port}`);
});
