ALTER TABLE event
    ALTER COLUMN access_code SET NOT NULL;

ALTER TABLE event
    ADD CONSTRAINT access_code_unique_constraint UNIQUE (access_code);