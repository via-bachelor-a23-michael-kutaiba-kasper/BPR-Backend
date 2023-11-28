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
