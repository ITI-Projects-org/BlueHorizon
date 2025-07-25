export interface BookingDTO {
  TenantId: number;
  UnitId: number;
  BookingDate: Date;
  CheckInDate: Date;
  CheckOutDate: Date;
  TotalPrice: number;
  PlatformComission: number;
  OwnerPayoutAmount: number;
  NumberOfGuests: number;
}
