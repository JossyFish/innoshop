'use client';
import { useCallback, useEffect, useLayoutEffect, useRef, useState } from "react";
import { Product } from "../../Models/Product";
import { ProductModel } from "../../Components/ProductModel/ProductModel";
import styles from "./products.module.css";
import { AddProduct, BulkProductOperation, ChangeProduct, GetMyProducts } from "../../Middlewares/productMiddleware";
import { AddProductModal } from "@/app/Components/AddProductModal/AddProductModal";
import { EditProductModal } from "@/app/Components/EditProductModal/EditProductModal";
import { ProductOperations } from "@/app/Enums/Operations";

export default function ProfileProducts() {
    const [products, setProducts] = useState<Product[]>([]);
    const [selectedProducts, setSelectedProducts] = useState<Set<string>>(new Set());
    const [productsCount, setProductsCount] = useState(0);
    const [loading, setLoading] = useState(false);
    const [hasMore, setHasMore] = useState(true);
    const [pageNumber, setPageNumber] = useState(1);
    const loadMoreRef = useRef<HTMLDivElement>(null);
    const observerRef = useRef<IntersectionObserver | null>(null);
    const [showAddModal, setShowAddModal] = useState(false);
    const [editingProduct, setEditingProduct] = useState<Product | null>(null);
    const isAllSelected = products.length > 0 && selectedProducts.size === products.length;
    const hasSelectedProducts = selectedProducts.size > 0;

    const fetchProducts = useCallback(async (isLoadMore = false) => {
        if (loading) return;
        
        setLoading(true);
        
        try {
            const currentPage = isLoadMore ? pageNumber + 1 : 1;
            const response = await GetMyProducts(currentPage);
            
            if (response.success && response.data) {
                if (response.data.products && Array.isArray(response.data.products)) {
                    if (isLoadMore) {
                        setProducts(prev => [...prev, ...response.data.products]);
                    } else {
                        setProducts(response.data.products);
                    }
                    
                    setProductsCount(response.data.totalCount);
                    setHasMore(currentPage < response.data.totalPages);
                    setPageNumber(currentPage);
                }
            } else {
                console.error('Failed to fetch products:', response.error);
            }
        } catch (error) {
            console.error('Error fetching products:', error);
        } finally {
            setLoading(false);
        }
    }, [loading, pageNumber]);

    useLayoutEffect(() => {
        fetchProducts(false);
    }, []);

    useLayoutEffect(() => {
        if (loading || !hasMore) return;

        observerRef.current = new IntersectionObserver(
            (entries) => {
                if (entries[0].isIntersecting) {
                    fetchProducts(true);
                }
            },
            { threshold: 0.1 }
        );

        if (loadMoreRef.current) {
            observerRef.current.observe(loadMoreRef.current);
        }

        return () => {
            if (observerRef.current) {
                observerRef.current.disconnect();
            }
        };
    }, [loading, hasMore, fetchProducts]);

    const toggleProductSelection = (productId: string) => {
        setSelectedProducts(prev => {
            const newSelection = new Set(prev);
            if (newSelection.has(productId)) {
                newSelection.delete(productId);
            } else {
                newSelection.add(productId);
            }
            return newSelection;
        });
    };

    const handleProductRightClick = (product: Product, e: React.MouseEvent) => {
        e.preventDefault();
        setEditingProduct(product);
    };

     const handleBulkOperation = async (operation: ProductOperations) => {
        if (selectedProducts.size === 0) return;
        
        const productIds = Array.from(selectedProducts);
        
        if (operation === ProductOperations.Delete) {
            const isConfirmed = confirm(`Delete ${selectedProducts.size} products?`);
            if (!isConfirmed) return;
        }

        if (operation === ProductOperations.Deactivate) {
            const isConfirmed = confirm(`Deactivate ${selectedProducts.size} products?`);
            if (!isConfirmed) return;
        }

        try {
            const result = await BulkProductOperation(productIds, operation);
            
            if (result.success) {
                let message = '';
                switch (operation) {
                    case ProductOperations.Delete:
                        message = `${selectedProducts.size} products deleted`;
                        break;
                    case ProductOperations.Deactivate:
                        message = `${selectedProducts.size} products deactivated`;
                        break;
                    case ProductOperations.Activate:
                        message = `${selectedProducts.size} products activated`;
                        break;
                }
                alert(message);
                
                setSelectedProducts(new Set());
                fetchProducts(false);
            } else {
                alert(result.error?.detail || `Failed to perform operation`);
            }
        } catch (error) {
            console.error('Bulk operation error:', error);
            alert('Operation failed');
        }
    };

    const selectAllProducts = () => {
        if (selectedProducts.size === products.length) {
            setSelectedProducts(new Set());
        } else {
            const allProductIds = products.map(product => product.id);
            setSelectedProducts(new Set(allProductIds));
        }
    };

     const deleteSelectedProducts = async () => {
        await handleBulkOperation(ProductOperations.Delete);
    };

    const deactivateSelectedProducts = async () => {
        await handleBulkOperation(ProductOperations.Deactivate);
    };

    const activateSelectedProducts = async () => {
        await handleBulkOperation(ProductOperations.Activate);
    };

    const handleAddProduct = async (productData: any) => {  
        const result = await AddProduct(productData);
        if (result.success) {
            fetchProducts(false);
        } else {
            alert(result.error?.detail || 'Failed to add product');
            throw new Error('Failed to add product');
        }
    };

    const handleUpdateProduct = async (productId: string, productData: any) => {
        const result = await ChangeProduct(productId, productData);
        if (result.success) {
            fetchProducts(false);
        } else {
            alert(result.error?.detail || 'Failed to update product');
            throw new Error('Failed to update product');
        }
    };

    return (
        <div className={styles.container}>
            <div className={styles.controlPanel}>
                <div className={styles.controlGroup}>
                    <button className={styles.controlButton} onClick={() => setShowAddModal(true)}>
                        + Add Product
                    </button>
                    <button 
                        className={`${styles.controlButton} ${isAllSelected ? styles.active : ''}`}
                        onClick={selectAllProducts}
                    >
                        {isAllSelected ? 'Deselect' : 'Select all'}
                    </button>
                    
                    <span className={styles.selectionInfo}>
                        Selected: {selectedProducts.size}
                    </span>
                </div>
                
                <div className={styles.actionGroup}>
                    <button 
                        className={styles.actionButton}
                        onClick={activateSelectedProducts}
                        disabled={!hasSelectedProducts}
                    >
                        Activate
                    </button>
                    
                    <button 
                        className={styles.warningButton}
                        onClick={deactivateSelectedProducts}
                        disabled={!hasSelectedProducts}
                    >
                        Deactivate
                    </button>
                    
                    <button 
                        className={styles.dangerButton}
                        onClick={deleteSelectedProducts}
                        disabled={!hasSelectedProducts}
                    >
                        Delete
                    </button>
                </div>
            </div>

            <div className={styles.content}>
                <div className={styles.header}>
                    <p className={styles.subtitle}>
                        {productsCount} products • {hasSelectedProducts ? `${selectedProducts.size} selected` : ''}
                    </p>
                </div>

                {products.length > 0 && (
                    <div className={styles.productsGrid}>
                        {products.map(product => (
                            <div 
                                key={product.id}
                                className={`${styles.productWrapper} ${
                                    selectedProducts.has(product.id) ? styles.selected : ''
                                }`}
                                onClick={() => toggleProductSelection(product.id)}
                                onContextMenu={(e) => handleProductRightClick(product, e)}
                            >
                                <ProductModel 
                                    product={product} 
                                    showCheckbox={true}
                                    isSelected={selectedProducts.has(product.id)}
                                />
                            </div>
                        ))}
                        
                        <div ref={loadMoreRef} className={styles.loadMoreTrigger} />
                    </div>
                )}

                {loading && (
                    <div className={styles.loading}>
                        Loading...
                    </div>
                )}

                {!loading && products.length === 0 && (
                    <div className={styles.emptyState}>
                        <p>Not found</p>
                    </div>
                )}
            </div>
            {showAddModal && (
                <AddProductModal onClose={() => setShowAddModal(false)} onAddProduct={handleAddProduct}/>)}
            {editingProduct && (
                 <EditProductModal product={editingProduct} onClose={() => setEditingProduct(null)} onUpdateProduct={handleUpdateProduct} />
            )}
        </div>
    );
}