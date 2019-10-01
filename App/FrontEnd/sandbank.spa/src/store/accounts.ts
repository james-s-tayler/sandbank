import Vue from 'vue';
import Vuex from 'vuex';
import { Account } from '@/account';

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
    },
});
