import Vue from 'vue';
import Vuex from 'vuex';
import authStoreModule from '@/store/auth';
import accountStoreModule from '@/store/accounts';

Vue.use(Vuex);

export const authStore = 'authStoreModule';
export const accountStore = 'accountStoreModule';

export default new Vuex.Store({
    modules: {
        authStoreModule,
        accountStoreModule,
    },
    state: {

    },
    mutations: {

    },
    getters: {

    },
    actions: {

    },
});
