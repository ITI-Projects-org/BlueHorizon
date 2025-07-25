export interface ISearchCriteria {
  selectedBedrooms?: string | null;
  selectedBathrooms?: string | null;
  minPrice?: number | null;
  maxPrice?: number | null;
  selectedVillage?: string | null; // ستبقى camelCase لأنها متغير محلي للفلتر
  selectedType?: string | null; // ستبقى string هنا حيث نتعامل معها كاسم
  sortOption?: string | null;
}
