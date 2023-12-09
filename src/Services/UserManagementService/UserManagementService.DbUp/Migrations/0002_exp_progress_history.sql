CREATE TABLE IF NOT EXISTS user_exp_progress
(
    id INT GENERATED ALWAYS AS IDENTITY,
    user_id VARCHAR,
    exp_gained BIGINT,
    date TIMESTAMPTZ,
    reviews_created INT, 
    events_hosted INT,
    PRIMARY KEY (id)
);