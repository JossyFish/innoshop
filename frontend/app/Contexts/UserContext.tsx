"use client";
import React, { createContext, useContext, useState, useEffect } from "react";

interface UserData {
    email: string;
    roles?: string[] | null;
}

interface UserContextType {
    user: UserData | null;
    setUser: (user: UserData | null) => void;
    isLoading: boolean;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider = ({ children }: { children: React.ReactNode }) => {
    const [user, setUserState] = useState<UserData | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const loadUser = () => {
            try {
                const savedUser = sessionStorage.getItem('user');
                if (savedUser) {
                    const userData = JSON.parse(savedUser);
                    setUserState(userData);
                }
            } catch (error) {
                console.error('Error parsing saved user:', error);
                sessionStorage.removeItem('user');
            } finally {
                setIsLoading(false);
            }
        };

        loadUser();
    }, []);

    const setUser = (userData: UserData | null) => {
        if (userData) {
            sessionStorage.setItem('user', JSON.stringify(userData));
        } else {
            sessionStorage.removeItem('user');
        }
        setUserState(userData);
    };

    const value: UserContextType = {
        user,
        setUser,
        isLoading
    };

    return (
        <UserContext.Provider value={value}>
            {children}
        </UserContext.Provider>
    );
};

export const useUserModel = () => {
    const context = useContext(UserContext);
    if (context === undefined) {
        throw new Error('useUserModel must be used within a UserProvider');
    }
    return context;
};