'use client';
import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useUserModel } from '../Contexts/UserContext';
import { ConfirmNewEmailLink, GetProfileData } from '../Middlewares/userMiddleware';
import styles from './confirmEmail.module.css';

export default function ConfirmEmailChangePage() {
    const router = useRouter();
    const { setUser } = useUserModel();
    const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
    const [message, setMessage] = useState('');

    useEffect(() => {
        const confirmEmailChange = async () => {
            const urlParams = new URLSearchParams(window.location.search);
            const token = urlParams.get('token');
            
            if (!token) {
                setStatus('error');
                setMessage('Invalid confirmation link');
                return;
            }

            try {
                const result = await ConfirmNewEmailLink(token);
                
                if (result.success) {
                    const profileResult = await GetProfileData();
                    
                    if (profileResult.success && profileResult.data) {
                        setUser({
                            email: profileResult.data.email,
                            roles: profileResult.data.roles || null
                        });
                        
                        setStatus('success');
                        setMessage('Email changed successfully. Redirecting to profile...');
                        
                        setTimeout(() => {
                            router.push('/profile');
                        }, 2000);
                    } else {
                        throw new Error('Failed to get user profile');
                    }
                } else {
                    throw new Error(result.error?.detail || 'Email confirmation failed');
                }
            } catch (error) {
                console.error('Email confirmation error:', error);
                setStatus('error');
                setMessage(error instanceof Error ? error.message : 'Email confirmation failed');
            }
        };

        confirmEmailChange();
    }, []);

    return (
        <div className={styles.container}>
            <div className={styles.card}>
                {status === 'loading' && (
                    <>
                        <div className={styles.spinner}></div>
                        <h2>Confirming Email Change...</h2>
                        <p>Please wait while we confirm your new email address</p>
                    </>
                )}
                
                {status === 'success' && (
                    <>
                        <h2>Email Changed Successfully</h2>
                        <p>{message}</p>
                    </>
                )}
                
                {status === 'error' && (
                    <>
                        <h2>Email Change Failed</h2>
                        <p>{message}</p>
                    </>
                )}
            </div>
        </div>
    );
}