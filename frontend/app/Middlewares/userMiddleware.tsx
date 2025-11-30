import { setCookie } from 'cookies-next';
import { changeActivity, changeEmail, changePassword, changeProfile, confirmCodeSignUp, confirmLinkSignUp, confirmNewEmail, confirmNewEmailLink, confirmNewPassword, deleteProfile, getProfileData, login, resetPassword, signUp } from '../Services/userService';

export const Login = async (email: string, password: string) => {
    const result = await login(email, password);
    if(result.success){
        SetTokenInCookie(result.data);
        return {
            success: true,
            data: result.data
        };
    } else{
        return result;
    }
}

export const SignUp = async (name:string, email:string, password:string) => {
    const result = await signUp(name, email, password);
    return result;
}

export const ConfirmCodeSignUp = async (email:string, code:string)=>{
    const result = await confirmCodeSignUp(email, code);
    if(result.success && result.data){
        SetTokenInCookie(result.data);
        return {
            success: true,
            data: result.data
        };
    } else{
        return result;
    }
}

export const ConfirmLinkSignUp = async (token: string) => {
    const result = await confirmLinkSignUp(token);
    
    if (result.success && result.data) {
        SetTokenInCookie(result.data);
        return result;
    } else {
        return result;
    }
}

export const ResetPassword = async (email:string) => {
    const result = await resetPassword(email);
    return result;
}

export const ConfirmNewPassword = async  (email:string, newPassword:string, code:string) => {
    const result = await confirmNewPassword(email, newPassword, code);
    return result;
}

export const DeleteProfile = async () => {
    const result = await deleteProfile();
    return result;
}

export const ChangePassword = async (currentPassword: string, newPassword: string) => {
    const result = await changePassword(currentPassword, newPassword);
    return result;
}

export const ChangeActivity= async (isActive: boolean) => {
    const result = await changeActivity(isActive);
    return result;
}

export const ChangeProfile = async (newName: string) => {
    const result = await changeProfile(newName);
    return result;
}

export const ChangeEmail = async (newEmale: string) => {
    const result = await changeEmail(newEmale);
    return result;
}
export const ConfirmNewEmail = async (code:string) => {
    const result = await confirmNewEmail(code);
    return result;
}

export const ConfirmNewEmailLink = async (token: string) => {
    const result = await confirmNewEmailLink(token);
    return result;
}

export const SetTokenInCookie = async (data:string) =>{
  setCookie('myCookie', data, {
          maxAge: 3600,
          secure: true, 
          sameSite: 'none'
      });
} 

export const GetProfileData = async () => {
    const result = await getProfileData();
    return result;
}
