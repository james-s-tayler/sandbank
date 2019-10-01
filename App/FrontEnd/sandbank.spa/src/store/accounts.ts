import Vue from 'vue';
import Vuex from 'vuex';
import { Account } from '@/account';
import { JwtHelper } from '@/jwt-helper';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
// tslint:disable-next-line: whitespace
      accounts: Array<Account>(),
      isAuthenticated: false ,
    },
    mutations: {
        addAccount(state, account: Account) {
          state.accounts.push(account);
        },
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
    },
    getters: {
        isAuthenticated: (state) => {
            return state.isAuthenticated;
        },
    },
    actions: {
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
    },
});
