'use client';
import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useUserModel } from '../Contexts/UserContext';
import { ConfirmLinkSignUp, GetProfileData } from '../Middlewares/userMiddleware';
import styles from './confirmRegister.module.css';

export default function ConfirmRegisterPage() {
    const router = useRouter();
    const { setUser } = useUserModel();
    const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
    const [message, setMessage] = useState('');

    useEffect(() => {
        const confirmRegistration = async () => {
            const urlParams = new URLSearchParams(window.location.search);
            const token = urlParams.get('token');
            
            if (!token) {
                setStatus('error');
                setMessage('Invalid confirmation link');
                return;
            }

            try {
                const result = await ConfirmLinkSignUp(token);
                
                if (result.success) {
                    const profileResult = await GetProfileData();
                    
                    if (profileResult.success && profileResult.data) {
                        setUser({
                            email: profileResult.data.email,
                            roles: profileResult.data.roles || null
                        });
                        
                        setStatus('success');
                        setMessage('Registration confirmed successfully! Redirecting to profile...');
                        
                        setTimeout(() => {
                            router.push('/profile');
                        }, 2000);
                    } else {
                        throw new Error('Failed to get user profile');
                    }
                } else {
                    throw new Error(result.error?.detail || 'Confirmation failed');
                }
            } catch (error) {
                console.error('Confirmation error:', error);
                setStatus('error');
                setMessage(error instanceof Error ? error.message : 'Confirmation failed');
            }
        };

        confirmRegistration();
    }, []); 

    return (
        <div className={styles.container}>
            <div className={styles.card}>
                {status === 'loading' && (
                    <>
                        <div className={styles.spinner}></div>
                        <h2>Confirming Registration...</h2>
                        <p>Please wait while we confirm your registration</p>
                    </>
                )}
                
                {status === 'success' && (
                    <>
                        <h2>Registration Confirmed</h2>
                        <p>{message}</p>
                    </>
                )}
                
                {status === 'error' && (
                    <>
                        <h2>Confirmation Failed</h2>
                        <p>{message}</p>
                    </>
                )}
            </div>
        </div>
    );
}