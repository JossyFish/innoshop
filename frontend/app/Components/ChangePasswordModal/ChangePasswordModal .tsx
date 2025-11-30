'use client';
import { useState } from 'react';
import styles from './changePassword.module.css';

interface ChangePasswordModalProps {
    onClose: () => void;
    onChangePassword: (currentPassword: string, newPassword: string) => Promise<void>;
}

export const ChangePasswordModal = ({ onClose, onChangePassword }: ChangePasswordModalProps) => {
    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [errors, setErrors] = useState<{
        currentPassword?: string;
        newPassword?: string;
        confirmPassword?: string;
    }>({});

    const validateForm = () => {
        const newErrors: { currentPassword?: string; newPassword?: string; confirmPassword?: string } = {};

        if (!currentPassword) {
            newErrors.currentPassword = 'Current password is required';
        }

        if (!newPassword) {
            newErrors.newPassword = 'New password is required';
        } else if (newPassword.length < 8) {
            newErrors.newPassword = 'Password must be at least 8 characters';
        }

        if (!confirmPassword) {
            newErrors.confirmPassword = 'Please confirm your password';
        } else if (newPassword !== confirmPassword) {
            newErrors.confirmPassword = 'Passwords do not match';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (!validateForm()) return;

        setLoading(true);
        try {
            await onChangePassword(currentPassword, newPassword);
        } catch (error) {
            console.error('Password change error:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleClose = () => {
        if (!loading) {
            onClose();
        }
    };

    return (
        <div className={styles.overlay}>
            <div className={styles.modal}>
                <header className={styles.header}>
                    <h2 className={styles.title}>Change Password</h2>
                    <button 
                        className={styles.closeButton}
                        onClick={handleClose}
                        disabled={loading}
                        aria-label="Close"
                    >
                        ×
                    </button>
                </header>

                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.inputGroup}>
                        <label className={styles.label}>Current Password</label>
                        <input
                            type="password"
                            value={currentPassword}
                            onChange={(e) => setCurrentPassword(e.target.value)}
                            className={`${styles.input} ${errors.currentPassword ? styles.error : ''}`}
                            placeholder="Enter current password"
                            disabled={loading}
                        />
                        {errors.currentPassword && (
                            <span className={styles.errorText}>{errors.currentPassword}</span>
                        )}
                    </div>

                    <div className={styles.inputGroup}>
                        <label className={styles.label}>New Password</label>
                        <input
                            type="password"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            className={`${styles.input} ${errors.newPassword ? styles.error : ''}`}
                            placeholder="Enter new password"
                            disabled={loading}
                        />
                        {errors.newPassword && (
                            <span className={styles.errorText}>{errors.newPassword}</span>
                        )}
                    </div>

                    <div className={styles.inputGroup}>
                        <label className={styles.label}>Confirm New Password</label>
                        <input
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            className={`${styles.input} ${errors.confirmPassword ? styles.error : ''}`}
                            placeholder="Confirm new password"
                            disabled={loading}
                        />
                        {errors.confirmPassword && (
                            <span className={styles.errorText}>{errors.confirmPassword}</span>
                        )}
                    </div>

                    <div className={styles.buttonGroup}>
                        <button
                            type="button"
                            className={styles.cancelButton}
                            onClick={handleClose}
                            disabled={loading}
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            className={styles.submitButton}
                            disabled={loading}
                        >
                            {loading ? 'Changing...' : 'Change Password'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};