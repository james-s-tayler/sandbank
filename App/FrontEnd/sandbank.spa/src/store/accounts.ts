import { Account } from '@/account';

const state = {
    accounts: Array<Account>(),
};

const getters = {
    accounts: (state) => {
        return state.accounts;
    },
};

const actions = {

};

const mutations = {
    addAccount(state, account: Account) {
        state.accounts.push(account);
    },
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations,
};
