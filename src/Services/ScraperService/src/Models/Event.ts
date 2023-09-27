export interface Event {
    title: string;
    host: string;
    url: string;
    location: Location;
    dateDisplay?: string;
    startDate?: Date;
    endDate?: Date;
    description?: string;
    images?: string[];
    numberOfAtendees?: number;
    price?: string;
}

export interface Location {
    country: string;
    city: string;
    postalCode: string;
    streetName: string;
    streetNumber: string;
    housenumber?: string;
    floor?: string;
}
