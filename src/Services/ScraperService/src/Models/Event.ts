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
    location: string;
    city: string;
    lat: number;
    lng: number;
    keywords: any[];
    attendees: string[];
    dateDisplay?: string;
    images?: string[];
    price?: string;
}
