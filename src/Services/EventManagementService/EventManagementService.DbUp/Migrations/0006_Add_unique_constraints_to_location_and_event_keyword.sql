ALTER TABLE event_keyword
    ADD CONSTRAINT unique_keyword
        UNIQUE (event_id, keyword);
ALTER TABLE location
    ADD CONSTRAINT unique_sub_premise
        UNIQUE (geolocation_lat, geolocation_lng, sub_premise); 