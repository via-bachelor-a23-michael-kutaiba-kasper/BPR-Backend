CREATE TABLE IF NOT EXISTS user_exp_progress
(
    id INT GENERATED ALWAYS AS IDENTITY,
    user_id VARCHAR,
    exp_gained BIGINT,
    datetime TIMESTAMPTZ,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS user_stats_history
(
    id INT GENERATED ALWAYS AS IDENTITY,
    user_id VARCHAR,
    reviews_created INT,
    events_hosted INT,
    datetime TIMESTAMPTZ,
    PRIMARY KEY (id)
);

CREATE INDEX date_index ON user_stats_history USING btree -- default is btree 
(
    user_id,
    datetime DESC
); -- Optimizes for the common scenario where we look up latest stats for a specific user

CREATE INDEX userid_index ON user_progress.progress
(
    user_id
);