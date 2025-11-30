'use client';
import { useState, useRef, useEffect } from 'react';
import styles from './confirmCode.module.css';

interface ConfirmCodeModelProps {
    onConfirm: (code: string) => void;
    onClose: () => void;
}

export const ConfirmCodeModel = ({ onConfirm, onClose }: ConfirmCodeModelProps) => {
    const [code, setCode] = useState(['', '', '', '', '', '']);
    const inputRefs = useRef<(HTMLInputElement | null)[]>([]);

    useEffect(() => {
        inputRefs.current = inputRefs.current.slice(0, 6);
    }, []);

    const handleChange = (index: number, value: string) => {
        if (value && !/^\d+$/.test(value)) return;

        const newCode = [...code];
        newCode[index] = value;
        setCode(newCode);

        if (value && index < 5) {
            inputRefs.current[index + 1]?.focus();
        }

        if (newCode.every(digit => digit !== '') && index === 5) {
            onConfirm(newCode.join(''));
        }
    };

    const handleKeyDown = (index: number, e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key === 'Backspace' && !code[index] && index > 0) {
            inputRefs.current[index - 1]?.focus();
        }

        if (e.key === 'ArrowLeft' && index > 0) {
            inputRefs.current[index - 1]?.focus();
        }
        if (e.key === 'ArrowRight' && index < 5) {
            inputRefs.current[index + 1]?.focus();
        }
    };

    const handlePaste = (e: React.ClipboardEvent) => {
        e.preventDefault();
        const pastedData = e.clipboardData.getData('text');
        const digits = pastedData.replace(/\D/g, '').split('').slice(0, 6);
        
        if (digits.length === 6) {
            setCode(digits);
            inputRefs.current[5]?.focus();
        }
    };

    const handleConfirm = () => {
        if (code.every(digit => digit !== '')) {
            onConfirm(code.join(''));
        }
    };

    const isCodeComplete = code.every(digit => digit !== '');

    return (
        <div className={styles.overlay}>
            <div className={styles.modal}>
                <header className={styles.header}>
                    <h2 className={styles.title}>Code confirmation</h2>
                    <button 
                        className={styles.closeButton}
                        onClick={onClose}
                        aria-label="Закрыть"
                    >
                        ×
                    </button>
                </header>

                <div className={styles.content}>
                    <p className={styles.description}>
                        Enter the 6-digit confirmation code sent to your email
                    </p>

                    <div className={styles.codeInputs}>
                        {code.map((digit, index) => (
                            <input
                                key={index}
                                ref={el => {
                                    inputRefs.current[index] = el;
                                }}
                                type="text"
                                inputMode="numeric"
                                maxLength={1}
                                value={digit}
                                onChange={(e) => handleChange(index, e.target.value)}
                                onKeyDown={(e) => handleKeyDown(index, e)}
                                onPaste={handlePaste}
                                className={styles.codeInput}
                                autoFocus={index === 0}
                            />
                        ))}
                    </div>

                    <button
                        className={`${styles.confirmButton} ${!isCodeComplete ? styles.disabled : ''}`}
                        onClick={handleConfirm}
                        disabled={!isCodeComplete}
                    >
                        Submit
                    </button>
                </div>
            </div>
        </div>
    );
};