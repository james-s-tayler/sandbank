import { JwtHelper } from '@/jwt-helper';
import { ActionContext } from 'vuex';

const state = {
    isAuthenticated: false ,
    locale: 'en-NZ',
};

const getters = {
    isAuthenticated: (authState: any) => {
        return authState.isAuthenticated;
    },
    locale: (authState: any) => {
        return authState.locale;
    },
};

const actions = {
    logout(context: ActionContext< any, any>) {
        context.commit('logout');
    },
    login(context: ActionContext< any, any>, jwtToken: string) {
        const parsedToken = new JwtHelper().decodeToken(jwtToken);
        if (typeof window !== 'undefined') {
            window.sessionStorage.setItem('authToken', jwtToken);
            window.sessionStorage.setItem('authTokenExpiration', parsedToken.exp);
        }
        context.commit('updateAuthStatus', true);
    },
};

const mutations = {
    updateAuthStatus(authState: any, isAuthenticated: boolean) {
        authState.isAuthenticated = isAuthenticated;
    },
    logout(authState: any) {
        if (typeof window !== 'undefined') {
            window.sessionStorage.removeItem('authToken');
            window.sessionStorage.removeItem('authTokenExpiration');
        }
        authState.isAuthenticated = false ;
    },
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations,
};
