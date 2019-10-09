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
    loadedHeaders: (accountState: any) => {
        return accountState.loadedHeaders;
    },
    loadedBalances: (accountState: any) => {
        return accountState.loadedBalances;
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

            const getBalancePromises = new Array< Promise< AxiosResponse< any>>>();
            const accountMap = new Map();

            context.state.accounts.forEach((account: Account) => {
                const getBalanceUrl = `/account/${account.id}/balance`;
                accountMap.set(getBalanceUrl, account);
                getBalancePromises.push(Axios.get(getBalanceUrl));
            });

            Axios.all(getBalancePromises).then((responses: Array< AxiosResponse< any>>) => {
                responses.forEach((response: AxiosResponse) => {
                    let url = (response.config.url || '').replace(response.config.baseURL as string, '');

                    const account = accountMap.get(url);
                    const balance = response.data;

                    context.commit('updateAccountBalance', { account, balance });
                });
            })
            .catch((error) => {
                accountMap.forEach((key: string, value: Account) => {
                    context.commit('updateAccountBalance',  { value, balance: 0 });
                });
            })
            .finally(() => {
                context.commit('finishedLoadingBalances');
            });
        }

        if (!context.state.loadedTransactions && includeTransactions) {
            context.state.accounts.forEach((account: Account) => {
                Axios.get(`/account/${account.id}/transaction`)
                .then((response: AxiosResponse) => {
                    const transactions = response.data;
                    context.commit('updateAccountTransactions', { account, transactions });
                })
                .catch((error) => {
                    context.commit('updateAccountTransactions',  { account, transactions: [] });
                });
            });
            context.commit('finishedLoadingTransactions');
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
    updateAccountTransactions(accountState: any, payload: any) {
        Vue.set(payload.account, 'transactions', payload.transactions);
    },
    finishedLoadingHeaders(accountState: any) {
        accountState.loadedHeaders = true;
    },
    finishedLoadingBalances(accountState: any) {
        accountState.loadedBalances = true;
    },
    finishedLoadingTransactions(accountState: any) {
        accountState.loadedTransactions = true;
    },
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations,
};
