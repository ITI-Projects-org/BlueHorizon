<<<<<<< HEAD
export interface OwnerVerificationDTO {
  id :number|null;
  ownerId :string|null;
  OwnerName :string|null;
  
  FrontNationalIdDocument :File  | null;
  BackNationalIdDocument: File|null;
  FrontNationalIdDocumentPath :string|null;
  BackNationalIdDocumentPath :string|null;

  DocumentType :number | string | null;
  UploadDate :Date|null;
  NationalId :string|null;
  VerificationNotes :string|null;
  VerificationStatus  :number | string | null;
  BankAccountDetails :string|null;
  UnitId :number;
  Title :string|null;
  Description :string|null;
  UnitType :number | string | null;
  Bedrooms :number;
  Bathrooms :number;
  Sleeps :number;
  DistanceToSea :number;
  BasePricePerNight :number;
  Address :string|null;
  VillageName :string|null;
  CreationDate :Date|null;
  AverageUnitRating :number;
  Contract :number | string | null;
  ContractPath :string|null;
  ContractFile : File | null;
  UnitAmenities:number[]|null;
}
=======
export interface OwnerVerificationDTO {
  id :number|null;
  ownerId :string|null;
  OwnerName :string|null;
  
  FrontNationalIdDocument :File  | null;
  BackNationalIdDocument: File|null;
  FrontNationalIdDocumentPath :string|null;
  BackNationalIdDocumentPath :string|null;

  DocumentType :number | string | null;
  UploadDate :Date|null;
  NationalId :string|null;
  VerificationNotes :string|null;
  VerificationStatus  :number | string | null;
  BankAccountDetails :string|null;
  UnitId :number;
  Title :string|null;
  Description :string|null;
  UnitType :number | string | null;
  Bedrooms :number;
  Bathrooms :number;
  Sleeps :number;
  DistanceToSea :number;
  BasePricePerNight :number;
  Address :string|null;
  VillageName :string|null;
  CreationDate :Date|null;
  AverageUnitRating :number;
  Contract :number | string | null;
  ContractPath :string|null;
  ContractFile : File | null;
  UnitAmenities:number[]|null;
}
>>>>>>> cb72b14bdaef95076a0d8c8478c03b994869df8b
