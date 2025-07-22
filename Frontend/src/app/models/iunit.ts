export interface Iunit {

    title: string;
    description: string;
    unitType: number; // 0: Apartment, 1: Chalet, 2: Villa
    bedrooms: number;
    bathrooms: number;
    sleeps: number;
    distanceToSea: number; // in meters
    basePricePerNight: number; // in EGP
    address: string;
    villageName: string; // e.g., Amwaj, Marassi
    amenityIds: number[]; // Array of amenity IDs
}
