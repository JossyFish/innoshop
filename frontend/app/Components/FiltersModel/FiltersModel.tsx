import { GetProductsFilters } from "@/app/Models/GetProductsFilters";
import styles from  "./filters.module.css";
import { useState } from "react";

interface FiltersModelProps {
    filters: GetProductsFilters;
    onClose: () => void;
    onFiltersChange: (filters: GetProductsFilters) => void;   
}

export const FiltersModel = ({filters, onClose, onFiltersChange }: FiltersModelProps) => {
    const [localFilters, setLocalFilters] = useState(filters);

    const outsideClick = (event: React.MouseEvent<HTMLDivElement>) => {
        if (event.target === event.currentTarget) {
            onClose();
        }
    }

    const ApplyFilters = () => {
        onFiltersChange(localFilters);
    };

    const ResetFilters = () => {
        const resetFilters = GetProductsFilters.reset();
        setLocalFilters(resetFilters);
        onFiltersChange(resetFilters);
    };


    const MinPriceChange = (value: string) => {
        const minPrice = value ? Number(value) : undefined;
        setLocalFilters(prev => GetProductsFilters.updateMinPrice(prev, minPrice));
    };

    const MaxPriceChange = (value: string) => {
        const maxPrice = value ? Number(value) : undefined;
        setLocalFilters(prev => GetProductsFilters.updateMaxPrice(prev, maxPrice));
    };

    const MinQuantityChange = (value: string) => {
        const minQuantity = value ? Number(value) : undefined;
        setLocalFilters(prev => GetProductsFilters.updateMinQuantity(prev, minQuantity));
    };

    const MinCreatedAtChange = (value: string) => {
        setLocalFilters(prev => ({ ...prev, minCreatedAt: value || undefined }));
    };

    const MaxCreatedAtChange = (value: string) => {
        setLocalFilters(prev => ({ ...prev, maxCreatedAt: value || undefined }));
    };

    return (
        <div className={styles.background} onClick={outsideClick}>
            <div className={styles.filters}>
                <header className={styles.header}>
                    <h3>Фильтры</h3>
                    <button onClick={onClose} className={styles.closeButton}>
                        <img src="/static/close.svg" alt="Закрыть" className={styles.closeImage}/>
                    </button>
                </header>
                
                <div className={styles.filterGroup}>
                    <label className={styles.label}>Цена</label>
                    <div className={styles.priceRange}>
                        <div className={styles.priceInput}>
                            <span className={styles.priceLabel}>От</span>
                            <input 
                                type="number"
                                value={localFilters.minPrice || ''}
                                onChange={(e) => MinPriceChange(e.target.value)}
                                placeholder="1"
                                className={`${styles.input} ${styles.singleInput}`}
                            />
                        </div>
                        <div className={styles.priceInput}>
                            <span className={styles.priceLabel}>До</span>
                            <input 
                                type="number"
                                value={localFilters.maxPrice || ''}
                                onChange={(e) => MaxPriceChange(e.target.value)}
                                placeholder="-"
                                className={`${styles.input} ${styles.singleInput}`}
                            />
                        </div>
                    </div>
                </div>

                <div className={styles.filterGroup}>
                    <label className={styles.label}>Валюта</label>
                    <select className={styles.select}>
                        <option value="">BYN</option>
                    </select>
                </div>

                <div className={styles.filterGroup}>
                    <label className={styles.label}>Минимальное наличие</label>
                    <input 
                        type="number"
                        value={localFilters.minQuantity || ''}
                        onChange={(e) => MinQuantityChange(e.target.value)}
                        placeholder="0"
                        className={styles.input}
                    />
                </div>

                <div className={styles.filterGroup}>
                    <label className={styles.label}>Дата создания</label>
                    <div className={styles.dateRange}>
                        <div className={styles.dateInput}>
                            <span className={styles.dateLabel}>От</span>
                            <input 
                                type="date"
                                value={localFilters.minCreatedAt || ''}
                                onChange={(e) => MinCreatedAtChange(e.target.value)}
                                className={`${styles.input} ${styles.singleInput}`}
                            />
                        </div>
                        <div className={styles.dateInput}>
                            <span className={styles.dateLabel}>До</span>
                            <input 
                                type="date"
                                value={localFilters.maxCreatedAt || ''}
                                onChange={(e) => MaxCreatedAtChange(e.target.value)}
                                className={`${styles.input} ${styles.singleInput}`}  
                            />
                        </div>
                    </div>
                </div>
                
                <div className={styles.actions}>
                    <button onClick={ResetFilters} className={styles.resetButton}>
                        Сбросить
                    </button>
                    <button  onClick={ApplyFilters} className={styles.applyButton}>
                        Применить
                    </button>
                </div>
            </div>
        </div>     
    );
};