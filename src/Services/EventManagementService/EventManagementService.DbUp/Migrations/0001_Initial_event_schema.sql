CREATE TABLE IF NOT EXISTS location
(
    id              INT GENERATED ALWAYS AS IDENTITY,
    street_number   VARCHAR,
    street_name     VARCHAR,
    sub_premise     VARCHAR,
    city            VARCHAR,
    postal_code     VARCHAR,
    country         VARCHAR,
    geolocation_lat DECIMAL NOT NULL,
    geolocation_lng DECIMAL NOT NULL,
    UNIQUE (geolocation_lat, geolocation_lng),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS category
(
    id   INT GENERATED ALWAYS AS IDENTITY,
    name VARCHAR NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS keyword
(
    id   INT GENERATED ALWAYS AS IDENTITY,
    name VARCHAR NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS event
(
    id                      BIGINT GENERATED ALWAYS AS IDENTITY,
    title                   VARCHAR     NOT NULL,
    start_date              TIMESTAMPTZ NOT NULL,
    end_date                TIMESTAMPTZ NOT NULL,
    created_date            TIMESTAMPTZ NOT NULL,
    is_private              BOOLEAN     NOT NULL,
    adult_only              BOOLEAN     NOT NULL,
    is_free                 BOOLEAN     NOT NULL,
    host_id                 VARCHAR     NOT NULL,
    max_number_of_attendees INT,
    last_update_date        TIMESTAMPTZ,
    url                     VARCHAR,
    description             VARCHAR,
    location_id             INT,
    FOREIGN KEY (location_id) REFERENCES location (id),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS image
(
    id       INT GENERATED ALWAYS AS IDENTITY,
    uri      VARCHAR NOT NULL,
    event_id BIGINT,
    FOREIGN KEY (event_id) REFERENCES event (id),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS event_category
(
    event_id    BIGINT,
    category_id INT,
    FOREIGN KEY (event_id) REFERENCES event (id),
    FOREIGN KEY (category_id) REFERENCES category (id)
);

CREATE TABLE IF NOT EXISTS keyword_category
(
    event_id BIGINT,
    keyword  INT,
    FOREIGN KEY (event_id) REFERENCES event (id),
    FOREIGN KEY (keyword) REFERENCES keyword (id)
);

CREATE TABLE IF NOT EXISTS event_attendee
(
    event_id BIGINT,
    user_id  VARCHAR,
    FOREIGN KEY (event_id) REFERENCES event (id)
);


