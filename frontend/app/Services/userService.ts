import { USER_URL } from '@/api-urls';
import { User } from '../Models/User';

export const login = async (email:string, password:string)=>{
    const response = await fetch(`${USER_URL}User/login`, {
        method: "POST",
        headers: {
          "content-type": "application/json",
        },
        body: JSON.stringify({
            email: email,   
            password: password
        }),
      });
    
      if (response.ok) {
          const data = await response.text();
          return { success: true, data };
      } else { 
          const errorData: ProblemDetails = await response.json();
          return { 
            success: false, 
            error: errorData,
            data: ""
          };
      }
};

export const signUp = async (name:string, email:string, password:string) => {
  const response = await fetch(`${USER_URL}User/register`, {
        method: "POST",
        headers: {
          "content-type": "application/json",
        },
        body: JSON.stringify({
            name: name,
            email: email,   
            password: password
        }),
      });
    
      if (response.ok) {
          return { success: true };
      } else { 
          const errorData: ProblemDetails = await response.json();
          return { 
            success: false, 
            error: errorData
          };
      }
}

export const confirmCodeSignUp = async (email:string, code:string) => {
  const response = await fetch(`${USER_URL}User/confirm-registration-code`, {
        method: "POST",
        headers: {
          "content-type": "application/json",
        },
        body: JSON.stringify({
            email: email,   
            confirmationCode: code
        }),
      });
    
      if (response.ok) {
          const data = await response.text();
          return { success: true, data };
      } else { 
          const errorData: ProblemDetails = await response.json();
          return { 
            success: false, 
            error: errorData
          };
      }
}

export const confirmLinkSignUp = async (token: string) => {
    const response = await fetch(`${USER_URL}User/confirm-registration-link?token=${encodeURIComponent(token)}`, {
        method: "GET",
        credentials: 'include',
    });
    
    if (response.ok) {
        const data = await response.text();
        return { success: true, data };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}

export const resetPassword = async (email:string) => {
  const response = await fetch(`${USER_URL}User/reset-password`, {
        method: "POST",
        headers: {
          "content-type": "application/json",
        },
        body: JSON.stringify({
            email: email
        }),
      });
    
      if (response.ok) {
          return { success: true };
      } else { 
          const errorData: ProblemDetails = await response.json();
          return { 
            success: false, 
            error: errorData
          };
      }
}

export const confirmNewPassword = async (email:string, newPassword:string, code:string) => {
  const response = await fetch(`${USER_URL}User/confirm-newPassword`, {
        method: "POST",
        headers: {
          "content-type": "application/json",
        },
        body: JSON.stringify({
            email: email,
            newPassword: newPassword,
            confirmationCode: code
        }),
      });
    
      if (response.ok) {
          return { success: true };
      } else { 
          const errorData: ProblemDetails = await response.json();
          return { 
            success: false, 
            error: errorData
          };
      }
}

export const getProfileData = async ()=>{
  const response = await fetch(`${USER_URL}User/profile`, {
    method: "GET",
    credentials: 'include',
  });
   if (response.ok) {
      const data: User = await response.json(); 
      return { success: true, data };
  }else {
      const errorData: ProblemDetails = await response.json();
      return { 
        success: false, 
        error: errorData
      };
  }
};

export const deleteProfile = async ()=>{
  const response = await fetch(`${USER_URL}User/delete`, {
    method: "DELETE",
    credentials: 'include',
  });
   if (response.ok) {
      return { success: true };
  }else {
      const errorData: ProblemDetails = await response.json();
      return { 
        success: false, 
        error: errorData
      };
  }
};

export const changePassword = async (currentPassword: string, newPassword: string) => {
    const response = await fetch(`${USER_URL}User/change-password`, {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        credentials: 'include',
        body: JSON.stringify({
            currentPassword: currentPassword,
            newPassword: newPassword
        }),
    });
    
    if (response.ok) {
        return { success: true };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}


export const changeActivity = async (isActive: boolean) => {
    const response = await fetch(`${USER_URL}User/change-active`, {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        credentials: 'include',
        body: JSON.stringify({
            isActive: isActive
        }),
    });
    
    if (response.ok) {
        return { success: true };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}

export const changeProfile = async (newName: string) => {
    const response = await fetch(`${USER_URL}User/change-profile`, {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        credentials: 'include',
        body: JSON.stringify({
            newName: newName
        }),
    });
    
    if (response.ok) {
        return { success: true };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}

export const changeEmail = async (newEmale: string) => {
    const response = await fetch(`${USER_URL}User/change-email`, {
        method: "POST",
        headers: {
            "content-type": "application/json",
        },
        credentials: 'include',
        body: JSON.stringify({
            newEmail: newEmale
        }),
    });
    
    if (response.ok) {
        return { success: true };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}

export const confirmNewEmail = async (code:string) => {
  const response = await fetch(`${USER_URL}User/confirm-newEmail-code`, {
        method: "POST",
        headers: {
          "content-type": "application/json",
        },
        credentials: 'include',
        body: JSON.stringify({
            confirmationCode: code
        }),
      });
    
      if (response.ok) {
          return { success: true };
      } else { 
          const errorData: ProblemDetails = await response.json();
          return { 
            success: false, 
            error: errorData
          };
      }
}

export const confirmNewEmailLink = async (token: string) => {
    const response = await fetch(`${USER_URL}User/confirm-newEmail-link?token=${encodeURIComponent(token)}`, {
        method: "GET",
        credentials: 'include',
    });
    
    if (response.ok) {
        return { success: true };
    } else { 
        const errorData: ProblemDetails = await response.json();
        return { 
            success: false, 
            error: errorData
        };
    }
}