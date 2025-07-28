export interface UnitVerificationDTO {
  id: number;
  documentType: number | string | null;
  uploadDate: Date | null;
  nationalId: string | null;
  verificationNotes: string | null;
  verificationStatus: number | string | null;
  bankAccountDetails: string | null;
  unitId: number;
  title: string | null;
  description: string | null;
  unitType: number | string | null;
  bedrooms: number;
  bathrooms: number;
  sleeps: number;
  distanceToSea: number;
  basePricePerNight: number;
  address: string | null;
  villageName: string | null;
  creationDate: Date | null;
  averageUnitRating: number;
  contract: number | string | null;
  contractPath: string | null;
  unitAmenities: number[] | null;
}
