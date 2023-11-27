ALTER TABLE event_keyword 
    ADD CONSTRAINT unique_keyword 
        UNIQUE (keyword);
ALTER TABLE location 
    ADD CONSTRAINT unique_sub_premise 
        UNIQUE (geolocation_lat, geolocation_lng, sub_premise); 