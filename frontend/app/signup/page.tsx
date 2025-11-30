'use client';
import {useState } from "react";
import { useUserModel } from "../Contexts/UserContext";
import { useRouter } from 'next/navigation';
import styles from  "./signup.module.css";
import { ConfirmCodeSignUp, SignUp } from "../Middlewares/userMiddleware";
import { ConfirmCodeModel } from "../Components/ConfirmCodeModal/ConfirmCodeModal";

export default function Signup() {
    const userContext = useUserModel();
    const router = useRouter();
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showConfirmCodeModel, setShowConfirmCodeModel] = useState(false);
    
    

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const result = await SignUp(name, email, password);
        
        if (result.success) {   
           setShowConfirmCodeModel(true);
        } else {
            alert(result.error?.detail || 'Login failed');
        }
    };

    const onConfirm = async (code: string) => {
        const result = await ConfirmCodeSignUp(email, code);
        if (result.success) {   
            const newUser = {
                    email: email,
                    roles: null
                };
            userContext?.setUser(newUser);
            router.push('/profile');
        } else {
            alert(result.error?.detail || 'Register failed');
        }
    };

return(
    <div className={styles.container}>
        <div className={styles.model}>
         <header className={styles.header}>
            <p className={styles.title}>Sign up</p>
         </header>
          <div className={styles.content}> 
                <div className={styles.inputGroup}>
                    <label className={styles.label}>Name</label>
                    <input 
                        type="name" 
                        placeholder="Enter name"
                        className={styles.input}
                        value={name} 
                        onChange={(e) => setName(e.target.value)}
                    />
                </div>
               <div className={styles.inputGroup}>
                    <label className={styles.label}>Email</label>
                    <input 
                        type="email" 
                        placeholder="Enter email"
                        className={styles.input}
                        value={email} 
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>
                
                <div className={styles.inputGroup}>
                    <label className={styles.label}>Password</label>
                    <input 
                        type="password" 
                        placeholder="Enter password"
                        className={styles.input}
                        value={password} 
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>

                <button className={styles.loginButton} onClick={handleSubmit}>
                    Sign Up
                </button>
          </div>
        </div>
        {showConfirmCodeModel && (
                    <ConfirmCodeModel onConfirm={onConfirm} onClose={() => setShowConfirmCodeModel(false)}/>
                )}
    </div>
);
}