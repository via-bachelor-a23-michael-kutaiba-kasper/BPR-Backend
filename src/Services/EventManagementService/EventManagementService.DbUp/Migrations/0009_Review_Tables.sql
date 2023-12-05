CREATE TABLE IF NOT EXISTS review
(
    id          INT GENERATED ALWAYS AS IDENTITY,
    rate        DECIMAL     NOT NULL,
    reviewer_id VARCHAR     NOT NULL,
    review_date TIMESTAMPTZ NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS event_review
(
    event_id  BIGINT,
    review_id INT,
    FOREIGN KEY (event_id) REFERENCES event (id),
    FOREIGN KEY (review_id) REFERENCES review (id)
);