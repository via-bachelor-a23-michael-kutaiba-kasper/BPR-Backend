TRUNCATE TABLE IF EXISTS event CASCADE;
create table if not exists event
(
    id          bigint GENERATED ALWAYS AS IDENTITY,
    title       varchar NOT NULL,
    url         varchar NOT NULL,
    location    varchar NOT NULL,
    description varchar,
    PRIMARY KEY (id)
)