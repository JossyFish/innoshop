import { ProductOperations } from "../Enums/Operations";
import { GetProductsFilters } from "../Models/GetProductsFilters";
import { addProduct, bulkProductOperation, changeProduct, getMyProducts, getProductById, getProducts } from "../Services/productService";

export const GetProducts = async (filters: GetProductsFilters) => {
    var products = await getProducts(filters);
    return products;
};

export const GetMyProducts = async (pageNumber: number = 1, pageSize: number = 20) => {
    const result = await getMyProducts(pageNumber, pageSize);
    return result;
}

export const AddProduct = async (productData: any) => {
    const result = await addProduct(productData);
    return result;
}

export const ChangeProduct = async (productId: string, productData: any) => {
    const result = await changeProduct(productId, productData);
    return result;
}

export const BulkProductOperation = async (productIds: string[], operation: ProductOperations) => {
    const result = await bulkProductOperation(productIds, operation);
    return result;
}

export const GetProductById = async (productId: string) => {
    const result = await getProductById(productId);
    return result;
}