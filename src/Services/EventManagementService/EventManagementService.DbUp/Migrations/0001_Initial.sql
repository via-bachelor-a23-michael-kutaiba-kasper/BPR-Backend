create table if not exists event
(
    id          bigint GENERATED ALWAYS AS IDENTITY,
    title       varchar NOT NULL,
    url         varchar NOT NULL,
    location    varchar NOT NULL,
    description varchar,
    UNIQUE (url),
    PRIMARY KEY (id)
)