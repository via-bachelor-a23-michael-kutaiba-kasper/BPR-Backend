namespace EventManagementService.Application.CreateEvents.Sql;

internal static class SqlCommands
{
    internal const string CreateTempTables =
        """"
        CREATE TEMP TABLE temp_location
        (
            street_number   VARCHAR,
            street_name     VARCHAR,
            sub_premise     VARCHAR,
            city            VARCHAR,
            postal_code     VARCHAR,
            country         VARCHAR,
            geolocation_lat DECIMAL,
            geolocation_lng DECIMAL
        );
        CREATE TEMP TABLE temp_event
        (
            title                   VARCHAR     NOT NULL,
            start_date              TIMESTAMPTZ NOT NULL,
            end_date                TIMESTAMPTZ NOT NULL,
            created_date            TIMESTAMPTZ NOT NULL,
            is_private              BOOLEAN     NOT NULL,
            adult_only              BOOLEAN     NOT NULL,
            is_paid                 BOOLEAN     NOT NULL,
            host_id                 VARCHAR     NOT NULL,
            event_code              VARCHAR NOT NULL, 
            max_number_of_attendees INT,
            last_update_date        TIMESTAMPTZ,
            url                     VARCHAR,
            description             VARCHAR,
            sub_premise             VARCHAR,
            geolocation_lat         DECIMAL,
            geolocation_lng         DECIMAL
        );
        CREATE TEMP TABLE temp_image
        (
            uri                     VARCHAR,
            url                     VARCHAR
        );
        CREATE TEMP TABLE temp_event_keyword
        (
            id                      INT,
            url                     VARCHAR
        );
        """";

    internal const string UpsertLocations =
        """
        MERGE INTO location lc USING temp_location tlc
        ON lc.geolocation_lng = tlc.geolocation_lng
        AND lc.geolocation_lat = tlc.geolocation_lat
        AND lc.sub_premise = tlc.sub_premise
        WHEN MATCHED
            THEN DO NOTHING
        WHEN NOT MATCHED
            THEN INSERT (
                         street_number,
                         street_name,
                         sub_premise,
                         city,
                         postal_code,
                         country,
                         geolocation_lat,
                         geolocation_lng
                         )
                VALUES (
                       tlc.street_number,
                       tlc.street_name,
                       tlc.sub_premise,
                       tlc.city,
                       tlc.postal_code,
                       tlc.country,
                       tlc.geolocation_lat,
                       tlc.geolocation_lng
                )
            
        """;

    internal const string UpsertEvents =
        """
        MERGE INTO event et USING
        (
            SELECT
                lc.id,
                tet.title,
                tet.start_date,
                tet.end_date,
                tet.created_date,
                tet.is_private,
                tet.adult_only,
                tet.is_paid,
                tet.host_id,
                tet.max_number_of_attendees,
                tet.last_update_date,
                tet.url,
                tet.description,
                tet.sub_premise,
                tet.geolocation_lat,
                tet.geolocation_lng,
                tet.category_id
            FROM location lc
            JOIN temp_location tlc
                ON lc.sub_premise = tlc.sub_premise
                       AND lc.geolocation_lat = tlc.geolocation_lat
                       AND lc.geolocation_lng = tlc.geolocation_lng
            JOIN temp_event tet
                ON tet.sub_premise = tlc.sub_premise
                          AND tet.geolocation_lat = tlc.geolocation_lat
                          AND tet.geolocation_lng = tlc.geolocation_lng
        ) as lc_u ON (lc_u.url = et.url)
        WHEN MATCHED
            THEN UPDATE
                 SET
                     title =  lc_u.title,
                     start_date =  lc_u.start_date,
                     end_date =  lc_u.end_date,
                     created_date =  lc_u.created_date,
                     is_private =  lc_u.is_private,
                     adult_only =  lc_u.adult_only,
                     is_paid =  lc_u.is_paid,
                     host_id =  lc_u.host_id,
                     last_update_date =  lc_u.last_update_date,
                     url =  lc_u.url,
                     description =  lc_u.description
        WHEN NOT MATCHED
            THEN INSERT (
                         title,
                         start_date,
                         end_date,
                         created_date,
                         is_private,
                         adult_only,
                         is_paid,
                         host_id,
                         max_number_of_attendees,
                         last_update_date,
                         url,
                         description,
                         location_id,
                         category_id
                         ) VALUES (
                        lc_u.title,
                        lc_u.start_date,
                        lc_u.end_date,
                        lc_u.created_date,
                        lc_u.is_private,
                        lc_u.adult_only,
                        lc_u.is_paid,
                        lc_u.host_id,
                        lc_u.last_update_date,
                        lc_u.url,
                        lc_u.description,
                        lc_u.id,
                        lc_u.category_id
                         )
        """;

    internal const string UpsertImage =
        """
        MERGE INTO image AS im
        USING (
        SELECT
            tim.uri,
            et.id
        FROM temp_image tim
        JOIN event et ON et.url = tim.url
            ) AS im_u ON (im_u.uri = im.uri)
            WHEN MATCHED THEN DO NOTHING
            WHEN NOT MATCHED
                THEN INSERT (uri, event_id)
                     VALUES (im_u.uri, im_u.id)
        """;

    internal const string UpsertEventKeyword =
        """
        MERGE INTO event_keyword AS etk
        USING (
            SELECT
                tec.key_id,
                et.url,
                et.id
            FROM temp_event_keyword tec
            JOIN event et ON et.url = tec.url
        ) AS etc_u ON (etc_u.key_id = etk.keyword)
        WHEN MATCHED THEN DO NOTHING
        WHEN NOT MATCHED
            THEN INSERT (event_id, keyword) VALUES (etc_u.id, etc_u.key_id)
        """;

    internal const string ImportLocationBinaryCopy =
        """
        COPY temp_location
            (
            street_number,
            street_name,
            sub_premise,
            city,
            postal_code,
            country,
            geolocation_lat,
            geolocation_lng
            )
            FROM STDIN (FORMAT BINARY );
        """;
    
    internal const string ImportEventBinaryCopy =
        """
        COPY temp_event
            (
            title,
            start_date,
            end_date,
            created_date,
            is_private,
            adult_only,
            is_paid,
            host_id,
            max_number_of_attendees,
            last_update_date,
            url,
            description,
            sub_premise,
            geolocation_lat,
            geolocation_lng
            )
            FROM STDIN (FORMAT BINARY );
        """;
    
    internal const string ImportImageBinaryCopy = 
        """
        COPY temp_image
        (
            uri,
            url
        )
        FROM STDIN (FORMAT BINARY);
        """;
    
    internal const string ImportKeywordBinaryCopy = 
        """
        COPY temp_event_keyword
        (
            id,
            url
        )
        FROM STDIN (FORMAT BINARY);
        """;
}