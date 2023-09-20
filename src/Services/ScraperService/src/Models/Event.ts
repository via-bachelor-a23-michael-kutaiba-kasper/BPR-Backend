export interface Event {
    title: string,
    url: string,
    startDate: Date,
    endDate: Date,
    location: Location
    description?: string,
    images?: string [],
    numberOfAtendees?: number,
    price?: number,
    currency?: string,
}

export interface Location {
    country: string,
    city: string,
    postalCode: string,
    streetName: string,
    streetNumber: string,
    housenumber?: string,
    floor?: string
}