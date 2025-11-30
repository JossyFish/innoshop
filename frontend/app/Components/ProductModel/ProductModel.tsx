import { Product } from "../../Models/Product";
import styles from  "./product.module.css";
import { Currency } from "../../Enums/Currency"; 
import { useRouter } from "next/navigation";

interface ProductModelProps {
    product: Product;
    showCheckbox?: boolean;
    isSelected?: boolean;
    onProductClick?: (productId: string) => void;
}

export const ProductModel = ({ product, showCheckbox = false, isSelected = false, onProductClick }: ProductModelProps) => {
    const currencyName = Currency[product.price.currency];
    const router = useRouter();
    
    const handleClick = () => {
        if (onProductClick) {
            onProductClick(product.id);
        }
    };

    return (
        <div className={`${styles.container} ${isSelected ? styles.selected : ''}`}>
            {showCheckbox && (
                <div className={styles.checkbox}>
                    <input 
                        type="checkbox" 
                        checked={isSelected}
                        readOnly
                    />
                </div>
            )}
            <div className={styles.product} onClick={handleClick}>
                <header className={styles.header}>
                    <p className={styles.name}>{product.name}</p>
                    <p className={styles.isActive}>{product.isActive ? '' : 'inactive'}</p>
                </header>
                <p className={styles.price}>{product.price.cost}{" " + currencyName}</p>
                <p className={styles.quantity}>{'в наличии: ' + product.quantity}</p>
            </div>  
        </div>
    );
};