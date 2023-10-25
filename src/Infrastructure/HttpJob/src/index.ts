const url = process.env["JOB_URL"];
const method = process.env["JOB_METHOD"];
const body = process.env["JOB_BODY"];

if (!url || !method) {
    console.error(
        "Missing method or url, please set JOB_URL and JOB_METHOD environment variables."
    );
    process.exit(1);
}

fetch(url, {
    body: body ?? body,
    method: method,
    headers: {
        "Content-Type": "application/json",
    },
})
    .then((res) => res.json())
    .then((data) =>
        console.log(
            `Job executed and received: ${JSON.stringify(data, null, 4)}`
        )
    )
    .catch((err) => console.error(err));
