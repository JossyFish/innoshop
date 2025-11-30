import { Product } from "./Product";

export interface GetProductsResponse {
    products: Product[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}
