import { JwtHelper } from '@/jwt-helper';

const state = {
    isAuthenticated: false ,
};

const getters = {
    isAuthenticated: (state) => {
        return state.isAuthenticated;
    },
};

const actions = {
    logout(context) {
        context.commit('logout');
    },
    login(context, jwtToken: string) {
        const parsedToken = new JwtHelper().decodeToken(jwtToken);
        if (typeof window !== 'undefined') {
            window.sessionStorage.setItem('authToken', jwtToken);
            window.sessionStorage.setItem('authTokenExpiration', parsedToken.exp);
        }
        context.commit('updateAuthStatus', true);
    },
};

const mutations = {
    updateAuthStatus(state, isAuthenticated: boolean) {
        state.isAuthenticated = isAuthenticated;
    },
    logout(state) {
        if (typeof window !== 'undefined') {
            window.sessionStorage.removeItem('authToken');
            window.sessionStorage.removeItem('authTokenExpiration');
        }
        state.isAuthenticated = false ;
    },
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations,
};
