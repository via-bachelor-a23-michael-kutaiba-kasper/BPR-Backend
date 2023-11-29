namespace EventManagementService.Application.ProcessExternalEvents.Sql;

internal static class SqlCommands
{
    internal const string CreateTempTables =
        """"
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
            max_number_of_attendees INT,
            last_update_date        TIMESTAMPTZ,
            url                     VARCHAR,
            description             VARCHAR,
            access_code             VARCHAR NOT NULL,
            category_id             INT,
            location                VARCHAR,
            city                    VARCHAR,
            geolocation_lat         DECIMAL,
            geolocation_lng         DECIMAL
        );
        CREATE TEMP TABLE temp_image
        (
            uri                             VARCHAR,
            access_code                     VARCHAR
        );
        CREATE TEMP TABLE temp_event_keyword
        (
            id                              INT,
            access_code                     VARCHAR
        );
        """";

    internal const string UpsertEvents = """
                                         MERGE INTO event e USING temp_event te
                                         ON (e.access_code = te.access_code)
                                         WHEN MATCHED THEN
                                             DO NOTHING
                                         WHEN NOT MATCHED THEN
                                             INSERT (
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
                                                 access_code,
                                                 category_id,
                                                 location,
                                                 city,
                                                 geolocation_lat,
                                                 geolocation_lng
                                             ) VALUES (
                                                 te.title,
                                                 te.start_date,
                                                 te.end_date,
                                                 te.created_date,
                                                 te.is_private,
                                                 te.adult_only,
                                                 te.is_paid,
                                                 te.host_id,
                                                 te.max_number_of_attendees,
                                                 te.last_update_date,
                                                 te.url,
                                                 te.description,
                                                 te.access_code,
                                                 te.category_id,
                                                 te.location,
                                                 te.city,
                                                 te.geolocation_lat,
                                                 te.geolocation_lng
                                             );
                                         """;

    internal const string UpsertImage =
        """
        MERGE INTO image AS im
        USING (
        SELECT
            tim.uri,
            et.id
        FROM temp_image tim
        JOIN event et ON et.access_code = tim.access_code
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
                tec.id AS key_id,
                et.access_code,
                et.id
            FROM temp_event_keyword tec
            JOIN event et ON et.access_code = tec.access_code
        ) AS etc_u ON (etc_u.key_id = etk.keyword)
        WHEN MATCHED THEN DO NOTHING
        WHEN NOT MATCHED
            THEN INSERT (event_id, keyword) VALUES (etc_u.id, etc_u.key_id)
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
            access_code,
            category_id,
            location,
            city,
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
            access_code
        )
        FROM STDIN (FORMAT BINARY);
        """;

    internal const string ImportKeywordBinaryCopy =
        """
        COPY temp_event_keyword
        (
            id,
            access_code
        )
        FROM STDIN (FORMAT BINARY);
        """;
}