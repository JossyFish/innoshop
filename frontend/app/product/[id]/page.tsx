'use client';
import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { Product } from '../../Models/Product';
import { GetProductById } from '../../Middlewares/productMiddleware';
import styles from './product.module.css';

export default function ProductPage() {
    const params = useParams();
    const router = useRouter();
    const [product, setProduct] = useState<Product | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchProduct = async () => {
            const productId = params.id as string;
            
            if (!productId) {
                setError('Product ID not found');
                setLoading(false);
                return;
            }

            try {
                const result = await GetProductById(productId);
                
                if (result.success && result.data) {
                    setProduct(result.data);
                } else {
                    setError(result.error?.detail || 'Failed to load product');
                }
            } catch (err) {
                setError('Error loading product');
                console.error('Product fetch error:', err);
            } finally {
                setLoading(false);
            }
        };

        fetchProduct();
    }, [params.id]);

    const handleBack = () => {
        router.back();
    };

    if (loading) {
        return (
            <div className={styles.container}>
                <div className={styles.loading}>Loading product...</div>
            </div>
        );
    }

    if (error || !product) {
        return (
            <div className={styles.container}>
                <div className={styles.error}>
                    <h2>Product not found</h2>
                    <p>{error}</p>
                </div>
            </div>
        );
    }

    function getCurrencySymbol(currency?: number): string {
        switch (currency) {
            case 1: return 'BYN';
            case 2: return 'USD';
            case 3: return 'EUR';
            case 4: return 'RUB';
            case 5: return 'CNY';
            default: return 'BYN';
        }
    }

    return (
        <div className={styles.container}>
            <div className={styles.header}>
                <button onClick={handleBack} className={styles.backButton}>
                    ← Back
                </button>
                <h1 className={styles.title}>Product Details</h1>
            </div>

            <div className={styles.productCard}>
                <div className={styles.productHeader}>
                    <h2 className={styles.productName}>{product.name}</h2>
                    <span className={`${styles.status} ${product.isActive ? styles.active : styles.inactive}`}>
                        {product.isActive ? 'Active' : 'Inactive'}
                    </span>
                </div>

                <div className={styles.productInfo}>
                    <div className={styles.infoSection}>
                        <h3 className={styles.sectionTitle}>Description</h3>
                        <p className={styles.description}>{product.description}</p>
                    </div>

                    <div className={styles.detailsGrid}>
                        <div className={styles.detailItem}>
                            <span className={styles.detailLabel}>Price:</span>
                            <span className={styles.detailValue}>
                                {product.price?.cost} {getCurrencySymbol(product.price?.currency)}
                            </span>
                        </div>

                        <div className={styles.detailItem}>
                            <span className={styles.detailLabel}>Quantity:</span>
                            <span className={styles.detailValue}>{product.quantity} items</span>
                        </div>

                        <div className={styles.detailItem}>
                            <span className={styles.detailLabel}>Created:</span>
                            <span className={styles.detailValue}>
                                {new Date(product.createdAt).toLocaleDateString()}
                            </span>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    );
}
