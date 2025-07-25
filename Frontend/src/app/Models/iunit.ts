
export interface IUnit {
  id:number|null,
  averageUnitRating:number|null,
  basePricePerNight:number,
  bathrooms:number|null,
  bedrooms:number|null,
  distanceToSea:number|null,
  sleeps:number|null,
  unitType:number|null,
  
  creationDate:Date|null,
  
  unitAmenities:string[]|null,
  ownerId:string|null,
  title:string|null,
  address:string|null,
  description:string|null,
  villageName:string|null,
  imageURL:string|null

}

