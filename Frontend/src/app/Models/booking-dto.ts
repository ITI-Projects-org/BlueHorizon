export interface BookingDTO {
  UnitId: number;
  CheckInDate: Date;
  CheckOutDate: Date;
  NumberOfGuests: number;
}

export interface BookingResponseDTO {
  id: number;
  unitId: number;
  tenantId: string;
  checkInDate: Date;
  checkOutDate: Date;
  numberOfGuests: number;
  bookingDate: Date;
  totalPrice: number;
  ownerPayoutAmount: number;
  platformComission: number;
  paymentStatus: string;
  unitReviewed: boolean;
  ownerReviewd: boolean;
  tenantName?: string; // Make sure this line is present and uncommented
  qrCodeUrl?: string;

  // Unit details (if populated)
  unit?: {
    id: number;
    title: string;
    address: string;
    villageName: string;
    unitType: string;
    basePricePerNight: number;
    ownerName: string;
  };
}
