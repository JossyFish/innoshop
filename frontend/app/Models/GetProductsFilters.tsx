
export interface GetProductsFilters{
    pageNumber?: number;
    pageSize?: number;
    name?: string;
    minPrice?: number;
    maxPrice?: number;
    currency?: number;
    minQuantity?: number;
    userIds?: string[];
    minCreatedAt?: string;
    maxCreatedAt?: string;
}

export const GetProductsFilters = {
  create: (init?: Partial<GetProductsFilters>): GetProductsFilters => ({
    pageNumber: 1,
    pageSize: 10,
    name: '',
    userIds: [],
    ...init
  }),

  updateName: (filters: GetProductsFilters, name: string): GetProductsFilters => ({
    ...filters,
    name
  }),

  updateMinPrice: (filters: GetProductsFilters, minPrice: number | undefined): GetProductsFilters => ({
    ...filters,
    minPrice
  }),

  updateMaxPrice: (filters: GetProductsFilters, maxPrice: number | undefined): GetProductsFilters => ({
    ...filters,
    maxPrice
  }),

  updateCurrency: (filters: GetProductsFilters, currency: number | undefined): GetProductsFilters => ({
    ...filters,
    currency
  }),

  updateMinQuantity: (filters: GetProductsFilters, minQuantity: number | undefined): GetProductsFilters => ({
    ...filters,
    minQuantity
  }),

  updateUserIds: (filters: GetProductsFilters, userIds: string[]): GetProductsFilters => ({
    ...filters,
    userIds
  }),

  updatePageNumber: (filters: GetProductsFilters, pageNumber: number): GetProductsFilters => ({
    ...filters,
    pageNumber
  }),

  reset: (): GetProductsFilters => ({
    pageNumber: 1,
    pageSize: 10,
    name: '',
    minPrice: undefined,
    maxPrice: undefined,
    currency: undefined,
    minQuantity: undefined,
    userIds: [],
    minCreatedAt: undefined,
    maxCreatedAt: undefined
  }),

  isEmpty: (filters: GetProductsFilters): boolean => {
    return !filters.name && 
           !filters.minPrice && 
           !filters.maxPrice && 
           !filters.currency && 
           !filters.minQuantity && 
           (!filters.userIds || filters.userIds.length === 0);
  }
};