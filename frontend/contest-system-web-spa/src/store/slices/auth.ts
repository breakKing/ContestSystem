import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ICurrentUser } from "../../models/auth/current-user";
import { ILoginData } from "../../models/auth/login";
import { ISignUpData } from "../../models/auth/sign-up";

export interface AuthState {
    currentUser: ICurrentUser | null;
    token: string | null;
    isInProgress: boolean;
    error: string | null;
};

const initialState: AuthState = {
    currentUser: null,
    token: null,
    isInProgress: false,
    error: null
};

export const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        login: (store, action: PayloadAction<ILoginData>) => {
            
        },
        signUp: (store, action: PayloadAction<ISignUpData>) => {

        }
    },
})

export const { login, signUp } = authSlice.actions;

export default authSlice.reducer;