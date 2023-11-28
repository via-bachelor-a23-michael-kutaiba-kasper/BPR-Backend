import { User } from "./User";

export interface Event {
    title: string;
    host: User;
    startDate?: Date;
    endDate?: Date;
    createdDate?: Date;
    lastUpdatedDate?: Date;
    isPrivate: boolean;
    adultsOnly: boolean;
    isPaid: boolean;
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
