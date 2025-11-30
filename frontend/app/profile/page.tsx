'use client';
import { useEffect, useLayoutEffect, useState } from "react";
import { useUserModel } from "../Contexts/UserContext";
import { useRouter } from 'next/navigation';
import { ChangeActivity, ChangeEmail, ChangePassword, ChangeProfile, ConfirmNewEmail, DeleteProfile, GetProfileData } from "../Middlewares/userMiddleware";
import { User } from "../Models/User";
import styles from  "./profile.module.css";
import { deleteCookie, getCookie } from "cookies-next";
import { ChangePasswordModal } from "../Components/ChangePasswordModal/ChangePasswordModal ";
import { ConfirmCodeModel } from "../Components/ConfirmCodeModal/ConfirmCodeModal";

export default function ProfilePage() {
    const { user, isLoading, setUser } = useUserModel();
    const router = useRouter();
    const [userData, setUserData] = useState<User>();
    const [hasFetched, setHasFetched] = useState(false); 
    const [isCheckingAuth, setIsCheckingAuth] = useState(true);
    const [showChangePasswordModal, setShowChangePasswordModal] = useState(false);
    const [editName, setEditName] = useState(''); 
    const [editEmail, setEditEmail] = useState('')
    const [showConfirmCodeModel, setShowConfirmCodeModel] = useState(false);

    useEffect(() => {
        if (isLoading) {
            return;
        }
        
        const myCookie = getCookie('myCookie');
        
        if (!myCookie || !user) {
            router.push('/login');
            return;
        } 

        setIsCheckingAuth(false);

        if (hasFetched) {
                return;
            }

        const fetchProfileData = async () => {

            try {
                const result = await GetProfileData();
                
                if (result.success && result.data) {
                    const updatedUser = {
                        ...user,
                        ...result.data,
                    };
                    setUser(updatedUser);
                    setUserData(result.data);
                    setEditName(result.data.name || ''); 
                    setEditEmail(result.data.email || '');
                    setHasFetched(true);
                } else {
                    alert(result.error?.detail || 'Failed to load profile data');
                }
            } catch (error) {
                console.error('Error fetching profile:', error);
                alert('An error occurred while loading profile');
                setTimeout(() => {
                    router.push('/');
                }, 1000);
            }
        };

        fetchProfileData();
    }, [user, isLoading, router, setUser]);

    const logOut = () =>{
        deleteCookie('myCookie');
        setUser(null);
        sessionStorage.removeItem('user');
        setTimeout(() => {
            router.push('/');
        }, 10);
    };

    if (isCheckingAuth || isLoading) {
        return (
            <div className={styles.loadingContainer}>
            </div>
        );
    }

      const handleResetPassword = () => {
        if(user){
        router.push(`/reset-password?email=${encodeURIComponent(user?.email)}`);
        }
        else{
            alert("Authentication problem");
            return;
        }
    }

    const deleteProfile = async () => {
    const isConfirmed = confirm("Вы уверены, что хотите удалить свой профиль?");
    
    if (isConfirmed) {
        const result = await DeleteProfile();
        if (result.success) {
            logOut();
        } else {
            alert(result.error?.detail || 'Failed to delete profile');
        }
    } else {
        alert("Удаление профиля отменено.");
    }
};

const changePassword = async (currentPassword: string, newPassword: string) => {
    const result = await ChangePassword(currentPassword, newPassword);
    if (result.success) {
        alert('Password changed has been changed');
        setShowChangePasswordModal(false);
    } else {
        alert(result.error?.detail || 'Failed to change password');
    }
}

const changeActivity = async () =>{
    const activity = userData?.isActive ? false : true;
    const result = await ChangeActivity(activity);
    if (result.success) {
        setUserData(prev => prev ? { ...prev, isActive: activity } : prev);
    }
    else{
        alert(result.error?.detail || 'Failed to change password');
    }

}

const changeName = async () => {
    const result = await ChangeProfile(editName);
    if (result.success) {
        setUserData(prev => prev ? { ...prev, name: editName } : prev);
        alert('Name updated');
    } else {
        alert(result.error?.detail || 'Failed to update name');
        if(userData){
        setEditName(userData.name);}
    }
};

const changeEmail = async () => {
    if(editEmail == user?.email){
        return;
    }
    const result = await ChangeEmail(editEmail);
    if(result.success){
        setShowConfirmCodeModel(true);
    }
    else{
        alert(result.error?.detail || 'Failed to change email');
        return;
    }
};

const onConfirm = async (code:string) => {
    const result = await ConfirmNewEmail(code);
    if (result.success) {   
        setUserData(prev => prev ? { ...prev, name: editName } : prev);
        if (user) {
            setUser({ ...user, email: editEmail });
        }
       setShowConfirmCodeModel(false);
    } else {
        alert(result.error?.detail || 'Register failed');
    }
};

return(
    <>
        <div className={styles.container}>
            <div className={styles.header}>
                <h1 className={styles.title}>Profile Settings</h1>
                <button className={styles.secondaryButton} onClick={logOut}>Log out</button>
            </div>

        <div className={styles.section}>
                <div className={styles.inputGroup}>
                    <label className={styles.label}>Name</label>
                    <div className={styles.inputWrapper}>
                        <input 
                            type="text" 
                            value={editName}
                            className={styles.input}
                            placeholder="Enter your full name"
                            onChange={(e) => { setEditName(e.target.value); }}
                        />
                        <button className={styles.primaryButton} onClick={changeName}> Update Name </button>
                    </div>
                </div>
                <div className={styles.inputGroup}>
                    <label className={styles.label}>Email</label>
                    <div className={styles.inputWrapper}>
                        <input 
                            type="email" 
                            value={editEmail}
                            className={styles.input}
                            placeholder="Enter your email"
                            onChange={(e) => setEditEmail(e.target.value)}
                        />
                        <button className={styles.primaryButton} onClick={changeEmail}> Update Email </button>
                    </div>
                </div>
                <div className={styles.statusGroup}>
                    <label className={styles.label}>Account Status</label>
                    <div className={styles.statusWrapper}>
                        <span className={`${styles.status} ${userData?.isActive ? styles.active : styles.inactive}`}>
                            {userData?.isActive ? 'Active' : 'Deactivated'}
                        </span>
                        <button className={userData?.isActive ? styles.warningButton : styles.successButton} onClick={changeActivity}>
                            {userData?.isActive ? 'Deactivate' : 'Activate'}
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <div className={styles.container}>
        <div className={styles.header}>
            <h3 className={styles.title}>Security</h3>
        </div>
        <div className={styles.section}>
            <div className={styles.changeGroup} onClick={() => setShowChangePasswordModal(true)}>
                <p>Change your password if you want</p>
                <button className={styles.secondaryButton}> Change</button>
            </div>
            <div className={styles.changeGroup}>
                <p>Or if you forgot password reset it</p>
                <button className={styles.secondaryButton} onClick={handleResetPassword}>Reset</button>
            </div>
            <div className={styles.changeGroup} onClick={deleteProfile}>
                <p>Delete your profile with all data</p>
                <button className={styles.warningButton}>Delete</button>
            </div>
        </div>  
        </div>
        {showChangePasswordModal && (
                <ChangePasswordModal onClose={() => setShowChangePasswordModal(false)} onChangePassword={changePassword} />
        )}
         {showConfirmCodeModel && (
                <ConfirmCodeModel onConfirm={onConfirm} onClose={() => setShowConfirmCodeModel(false)}/>
            )}
    </>
);
}
