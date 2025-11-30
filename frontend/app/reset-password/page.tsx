'use client';
import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import styles from './resetPassword.module.css';
import { ConfirmCodeModel } from '../Components/ConfirmCodeModal/ConfirmCodeModal';
import { ConfirmNewPassword, ResetPassword } from '../Middlewares/userMiddleware';

export default function ResetPasswordPage() {
    const router = useRouter();
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [showConfirmCodeModel, setShowConfirmCodeModel] = useState(false);
    const [email, setEmail] = useState('');
    const [errors, setErrors] = useState<{
        newPassword?: string;
        confirmPassword?: string;
    }>({});

    useEffect(() => {
        const urlParams = new URLSearchParams(window.location.search);
        const emailFromUrl = urlParams.get('email');
        if (emailFromUrl) {
            setEmail(emailFromUrl);
        }
    }, []);

    const validateForm = () => {
        const newErrors: { newPassword?: string; confirmPassword?: string } = {};

        if (!newPassword) {
            newErrors.newPassword = 'Password is required';
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
        if (validateForm()) {
            const result = await ResetPassword(email);
            
            if (result.success) {   
                setShowConfirmCodeModel(true);
            } else {
                alert(result.error?.detail + " P.S. Do not forget to enter email on login page" || 'Server error');
            }
        }
    };

    const onConfirm = async (code: string) => {
        try {
            const result = await ConfirmNewPassword(email, confirmPassword, code)
            if(result.success){
                router.back(); 
            }
            else{
                alert(result.error?.detail || 'Wrong code');
            }
        } catch (error) {
            alert('Password reset failed. Please try again.');
        }
    };

    return (
        <div className={styles.container}>
            <div className={styles.card}>
                <header className={styles.header}>
                    <h1 className={styles.title}>Reset Password</h1>
                    <p className={styles.subtitle}>Create a new password for your account</p>
                </header>

                {email && (
                    <div className={styles.emailInfo}>
                        Resetting password for: <strong>{email}</strong>
                    </div>
                )}

                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.inputGroup}>
                        <label className={styles.label}>New Password</label>
                        <input
                            type="password"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            className={`${styles.input} ${errors.newPassword ? styles.error : ''}`}
                            placeholder="Enter new password"
                        />
                        {errors.newPassword && (
                            <span className={styles.errorText}>{errors.newPassword}</span>
                        )}
                    </div>

                    <div className={styles.inputGroup}>
                        <label className={styles.label}>Confirm Password</label>
                        <input
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            className={`${styles.input} ${errors.confirmPassword ? styles.error : ''}`}
                            placeholder="Confirm new password"
                        />
                        {errors.confirmPassword && (
                            <span className={styles.errorText}>{errors.confirmPassword}</span>
                        )}
                    </div>

                    <button type="submit" className={styles.submitButton}>
                        Reset Password
                    </button>
                </form>

                <div className={styles.footer}>
                    <button 
                        type="button" 
                        className={styles.backButton}
                        onClick={() => router.back()}
                    >
                        Back to Login
                    </button>
                </div>
            </div>

            {showConfirmCodeModel && (
                <ConfirmCodeModel 
                    onConfirm={onConfirm} 
                    onClose={() => setShowConfirmCodeModel(false)}
                />
            )}
        </div>
    );
}