'use client';
import { useState } from 'react';
import styles from './addProduct.module.css';

interface AddProductModalProps {
    onClose: () => void;
    onAddProduct: (productData: any) => Promise<void>;
}

export const AddProductModal = ({ onClose, onAddProduct }: AddProductModalProps) => {
    const [formData, setFormData] = useState({
        name: '',
        description: '',
        cost: '',
        currency: 'BYN', 
        quantity: '',
        isActive: true
    });
    const [loading, setLoading] = useState(false);
    const [errors, setErrors] = useState<{[key: string]: string}>({});

    const validateForm = () => {
        const newErrors: {[key: string]: string} = {};

        if (!formData.name.trim()) newErrors.name = 'Name is required';
        if (!formData.description.trim()) newErrors.description = 'Description is required';
        if (!formData.cost || parseFloat(formData.cost) <= 0) newErrors.cost = 'Valid cost is required';
        if (!formData.quantity || parseInt(formData.quantity) < 0) newErrors.quantity = 'Valid quantity is required';

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (!validateForm()) return;

        setLoading(true);
        try {
            await onAddProduct({
                name: formData.name,
                description: formData.description,
                cost: parseFloat(formData.cost),
                currency: 1, // BYN = 1 из enum
                quantity: parseInt(formData.quantity),
                isActive: formData.isActive
            });
            onClose();
        } catch (error) {
            console.error('Error adding product:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (field: string, value: string | boolean) => {
        setFormData(prev => ({ ...prev, [field]: value }));
        if (errors[field]) {
            setErrors(prev => ({ ...prev, [field]: '' }));
        }
    };

    return (
        <div className={styles.overlay}>
            <div className={styles.modal}>
                <header className={styles.header}>
                    <h2 className={styles.title}>Add New Product</h2>
                    <button 
                        className={styles.closeButton}
                        onClick={onClose}
                        disabled={loading}
                    >
                        ×
                    </button>
                </header>

                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.inputGroup}>
                        <label className={styles.label}>Product Name *</label>
                        <input
                            type="text"
                            value={formData.name}
                            onChange={(e) => handleChange('name', e.target.value)}
                            className={`${styles.input} ${errors.name ? styles.error : ''}`}
                            placeholder="Enter product name"
                            disabled={loading}
                        />
                        {errors.name && <span className={styles.errorText}>{errors.name}</span>}
                    </div>

                    <div className={styles.inputGroup}>
                        <label className={styles.label}>Description *</label>
                        <textarea
                            value={formData.description}
                            onChange={(e) => handleChange('description', e.target.value)}
                            className={`${styles.textarea} ${errors.description ? styles.error : ''}`}
                            placeholder="Enter product description"
                            rows={3}
                            disabled={loading}
                        />
                        {errors.description && <span className={styles.errorText}>{errors.description}</span>}
                    </div>

                    <div className={styles.row}>
                        <div className={styles.inputGroup}>
                            <label className={styles.label}>Cost *</label>
                            <div className={styles.costInputWrapper}>
                                <input
                                    type="number"
                                    step="0.01"
                                    value={formData.cost}
                                    onChange={(e) => handleChange('cost', e.target.value)}
                                    className={`${styles.input} ${errors.cost ? styles.error : ''}`}
                                    placeholder="0.00"
                                    disabled={loading}
                                />
                                <span className={styles.currencySymbol}>BYN</span>
                            </div>
                            {errors.cost && <span className={styles.errorText}>{errors.cost}</span>}
                        </div>

                        <div className={styles.inputGroup}>
                            <label className={styles.label}>Quantity *</label>
                            <input
                                type="number"
                                value={formData.quantity}
                                onChange={(e) => handleChange('quantity', e.target.value)}
                                className={`${styles.input} ${errors.quantity ? styles.error : ''}`}
                                placeholder="0"
                                disabled={loading}
                            />
                            {errors.quantity && <span className={styles.errorText}>{errors.quantity}</span>}
                        </div>
                    </div>

                    <div className={styles.checkboxGroup}>
                        <label className={styles.checkboxLabel}>
                            <input
                                type="checkbox"
                                checked={formData.isActive}
                                onChange={(e) => handleChange('isActive', e.target.checked)}
                                className={styles.checkbox}
                                disabled={loading}
                            />
                            Active product
                        </label>
                    </div>

                    <div className={styles.buttonGroup}>
                        <button
                            type="button"
                            className={styles.cancelButton}
                            onClick={onClose}
                            disabled={loading}
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            className={styles.submitButton}
                            disabled={loading}
                        >
                            {loading ? 'Adding...' : 'Add Product'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};