type Job = {
    url: string;
    method: string;
    body: any;
};

async function executeJob(job: Job, incrementer: () => void) {
    let res: Response;
    if (job.body) {
        res = await fetch(job.url, {
            method: job.method,
            body: JSON.parse(job.body),
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

async function executeJobs(): Promise<number> {
    const jobs = process.env["JOBS"]?.trim();
    const jobsParsed = JSON.parse(jobs ?? "[]") as Job[];
    console.log(jobsParsed);
    let exectuedJobs = 0;
    await Promise.all(
        jobsParsed.map((job) => executeJob(job, () => exectuedJobs++))
    );

    return exectuedJobs;
}

executeJobs()
    .then((jobsExecuted) => console.log(`Executed ${jobsExecuted} jobs`))
    .catch((err) => console.error(err));
