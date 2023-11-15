ALTER TABLE event
    RENAME is_free TO is_paid;
ALTER TABLE event
    ADD COLUMN host_id     VARCHAR,
    ADD COLUMN event_code  VARCHAR UNIQUE,
    ADD COLUMN category_id INT;

ALTER TABLE event
    ADD CONSTRAINT fk_category
        FOREIGN KEY (category_id)
            REFERENCES category (id);   

DROP TABLE IF EXISTS event_category CASCADE;