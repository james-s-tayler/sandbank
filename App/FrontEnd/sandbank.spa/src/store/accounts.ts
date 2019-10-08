import Vue from 'vue';
import { Account } from '@/account';
import Axios, { AxiosResponse } from 'axios';
import { ActionContext } from 'vuex';

const state = {
    loadedHeaders: false ,
    loadedBalances: false ,
    loadedTransactions: false ,
    accounts: Array< Account>(),
};

const getters = {
    accounts: (accountState: any) => {
        return accountState.accounts;
    },
};

const actions = {
    async getAccounts(context: ActionContext< any, any>, includeBalances: boolean, includeTransactions: boolean) {
        if (!context.state.loadedHeaders) {

            const accounts: Account[] = await Axios.get('/account')
            .then((response: AxiosResponse) => {
                return response.data;
            })
            .catch((error) => alert(error));
            accounts.forEach((account: Account) => {
                context.commit('addAccount', account);
            });
            context.commit('finishedLoadingHeaders');
        }

        if (!context.state.loadedBalances && includeBalances) {
            context.state.accounts.forEach((account: Account) => {
                Axios.get(`/account/${account.id}/balance`)
                .then((response: AxiosResponse) => {
                    const balance = response.data;
                    context.commit('updateAccountBalance', { account, balance });
                })
                .catch((error) => {
                    context.commit('updateAccountBalance',  { account, balance: 0 });
                });
            });
            context.commit('finishedLoadingBalances');
        }
    },
};

const mutations = {
    addAccount(accountState: any, account: Account) {
        accountState.accounts.push(account);
    },
    updateAccountBalance(accountState: any, payload: any) {
        Vue.set(payload.account, 'balance', payload.balance);
    },
    finishedLoadingHeaders(accountState: any) {
        accountState.loadedHeaders = true;
    },
    finishedLoadingBalances(accountState: any) {
        accountState.loadedBalances = true;
    },
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations,
};
