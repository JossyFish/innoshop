import { Price } from "./Price";

export interface Product {
    id: string;
    name: string;
    description: string;
    price: Price;
    quantity: number;
    isActive: boolean;
    userId: string;
    createdAt: string;
}