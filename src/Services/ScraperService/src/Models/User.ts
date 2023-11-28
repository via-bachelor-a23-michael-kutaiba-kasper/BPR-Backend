export interface User {
    userId: string;
    displayName?: string;
    photoUrl?: string;
    dateOfBirth: Date;
    lastSeenOnline?: Date;
    creationDate: Date;
}
