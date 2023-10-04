create table if not exists Event
(
    id          bigint GENERATED ALWAYS AS IDENTITY,
    title       varchar NOT NULL,
    url         varchar NOT NULL,
    location    varchar NOT NULL,
    description varchar,

    PRIMARY KEY (id)
)