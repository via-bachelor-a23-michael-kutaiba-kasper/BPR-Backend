export interface Event {
    title: string;
    host: string;
    startDate?: Date;
    endDate?: Date;
    createdDate?: Date;
    lastUpdatedDate?: Date;
    isPrivate: boolean;
    adultsOnly: boolean;
    isPaid: boolean;
    hostId: string;
    maxNumberOfAttendees?: number;
    url: string;
    description?: string;
    location: Location;
    keywords: any[];
    attendees: string[];
    dateDisplay?: string;
    images?: string[];
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
