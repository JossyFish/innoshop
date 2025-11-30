import  { GetProductsFilters }  from '@/app/Models/GetProductsFilters';
import { PRODUCT_URL } from '@/api-urls';
import { GetProductsResponse } from '../Models/GetProductResponse';
import { ProductOperations } from '../Enums/Operations';
import { Product } from '../Models/Product';

export const getProducts = async (filters: GetProductsFilters) => {

    const urlParams = new URLSearchParams();
    
    if (filters.pageNumber) urlParams.append('pageNumber', filters.pageNumber.toString());
    if (filters.pageSize) urlParams.append('pageSize', filters.pageSize.toString());
    if (filters.name) urlParams.append('name', filters.name);
    if (filters.minPrice) urlParams.append('minPrice', filters.minPrice.toString());
    if (filters.maxPrice) urlParams.append('maxPrice', filters.maxPrice.toString());
    if (filters.currency) urlParams.append('currency', filters.currency.toString());
    if (filters.minQuantity) urlParams.append('minQuantity', filters.minQuantity.toString());

    if (filters.userIds) {
        filters.userIds.forEach(id => urlParams.append('userIds', id));
    }
    
    if (filters.minCreatedAt) {
        const minDate = new Date(filters.minCreatedAt);
        urlParams.append('minCreatedAt', minDate.toISOString());    
    }
    
    if (filters.maxCreatedAt) {
        const maxDate = new Date(filters.maxCreatedAt);
        urlParams.append('maxCreatedAt', maxDate.toISOString()); 
    }

    const response = await fetch(`${PRODUCT_URL}Product?${urlParams.toString()}`,{
        method: 'GET',
        headers: {
            'Content-Type':'application/json',
        },
        credentials:'include',
    });
    if(response.ok)
    {
        const data: GetProductsResponse = await response.json(); 
        return { success: true, data };
    }
    else{
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
          };
    }
};


export const getMyProducts = async (pageNumber: number = 1, pageSize: number = 20) => {
    
    const urlParams = new URLSearchParams({
        pageNumber: pageNumber.toString(),
        pageSize: pageSize.toString()
    });

    const response = await fetch(`${PRODUCT_URL}Product/my-products?${urlParams.toString()}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    });
    
    if (response.ok) {
        const data: GetProductsResponse = await response.json();
        return { success: true, data };
    } else {
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}

export const addProduct = async (productData: any) => {
    const response = await fetch(`${PRODUCT_URL}Product/add`, {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        credentials: 'include',
        body: JSON.stringify(productData),
    });
    
    if (response.ok) {
        return { success: true };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}

export const changeProduct = async (productId: string, productData: any) => {
    const response = await fetch(`${PRODUCT_URL}Product/change`, {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        credentials: 'include',
        body: JSON.stringify({
            productId: productId,
            ...productData
        }),
    });
    
    if (response.ok) {
        return { success: true };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}

export const bulkProductOperation = async (productIds: string[], operation: ProductOperations) => {
    const response = await fetch(`${PRODUCT_URL}Product/bulk`, {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        credentials: 'include',
        body: JSON.stringify({
            productIds: productIds,
            operation: operation
        }),
    });
    
    if (response.ok) {
        return { success: true };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}

export const getProductById = async (productId: string) => {
    const response = await fetch(`${PRODUCT_URL}Product/${productId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    });
    
    if (response.ok) {
        const data: Product = await response.json();
        return { success: true, data };
    } else {
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}