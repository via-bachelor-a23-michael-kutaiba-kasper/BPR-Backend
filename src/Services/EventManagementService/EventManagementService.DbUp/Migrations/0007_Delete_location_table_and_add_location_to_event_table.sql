DROP TABLE IF EXISTS location CASCADE;

ALTER TABLE IF EXISTS event
    DROP COLUMN location_id CASCADE ,
    ADD COLUMN location        VARCHAR,
    ADD COLUMN city            VARCHAR NOT NULL DEFAULT '',
    ADD COLUMN geolocation_lat DECIMAL NOT NULL DEFAULT 0,
    ADD COLUMN geolocation_lng DECIMAL NOT NULL DEFAULT 0;