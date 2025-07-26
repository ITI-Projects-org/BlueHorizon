export interface ReviewDTO
{
    unitId:number,
    bookingId :number,
    rating:number,
    comment :string|null,
    reviewStatus : number,
    tenantName:string|null,
    reviewDate:Date|null

}