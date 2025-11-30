'use client';
import { useLayoutEffect, useState } from "react";
import { useUserModel } from "../Contexts/UserContext";
import { useRouter } from 'next/navigation';
import styles from  "./login.module.css";
import Link from "next/link";
import { Login } from "../Middlewares/userMiddleware";

export default function LogIn() {
    const userContext = useUserModel();
    const router = useRouter();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const result = await Login(email, password);
        
        if (result.success) {   
            const newUser = {
                    email: email,
                    roles: null
                };
            userContext?.setUser(newUser);
            router.push('/profile');
        } else {
            alert(result.error?.detail || 'Login failed');
        }
    };

    const handleForgotPassword = () => {
        if(!email)
        {
            alert("Enter en email before resetting password");
            return;
        }
        router.push(`/reset-password?email=${encodeURIComponent(email)}`);
    }


return(
    <div className={styles.container}>
        <div className={styles.model}>
         <header className={styles.header}>
            <p className={styles.title}>Log in</p>
         </header>
          <div className={styles.content}> 
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
                    Sign In
                </button>
          </div>
          <div className={styles.footer}>
            <p className={styles.link} onClick={handleForgotPassword}>Forgot your password? Reset it.</p> 
            <Link className = {styles.link} href="/signup">Don't have an account yet? <u>Sign up.</u></Link>
          </div>
        </div>
    </div>
);
}