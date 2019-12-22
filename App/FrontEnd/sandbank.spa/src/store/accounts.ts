import Vue from 'vue';
import { Account } from '@/account';
import Axios, { AxiosResponse } from 'axios';
import { ActionContext } from 'vuex';
import { PostPaymentRequest } from '@/models/requests/post-payment-request';
import { LoadTransactionsRequest } from '@/models/requests/load-transactions-request';

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
    loadedAccounts: (accountState: any) => {
        return accountState.loadedHeaders && accountState.loadedBalances;
    },
};

const actions = {
    async getTransactions(context: ActionContext< any, any>, payload: LoadTransactionsRequest) {
        const dateRange = payload.range ? `?from=${payload.range[0]}&to=${payload.range[1]}` : '';
        Axios.get(`/account/${payload.account.id}/transaction` + dateRange)
        .then((response: AxiosResponse) => {
            const transactions = response.data;
            context.commit('updateAccountTransactions', { account: payload.account, transactions });
        })
        .catch((error) => {
            context.commit('updateAccountTransactions',  { account: payload.account, transactions: [] });
        });
    },
    async getAccounts(context: ActionContext< any, any>, payload: any) {
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

        if (!context.state.loadedBalances && payload.includeBalances) {

            const getBalancePromises = new Array< Promise< AxiosResponse< any>>>();
            const accountMap = new Map();

            context.state.accounts.forEach((account: Account) => {
                const getBalanceUrl = `/account/${account.id}/balance`;
                accountMap.set(getBalanceUrl, account);
                getBalancePromises.push(Axios.get(getBalanceUrl));
            });

            Axios.all(getBalancePromises).then((responses: Array< AxiosResponse< any>>) => {
                responses.forEach((response: AxiosResponse) => {
                    const url = (response.config.url || '').replace(response.config.baseURL as string, '');

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

        if (!context.state.loadedTransactions && payload.includeTransactions) {
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
    async transfer(context: ActionContext< any, any>, paymentRequest: PostPaymentRequest) {
        Axios.post('/payment', paymentRequest).then(() => {
            // should modify this method just to refresh a single account / balance / transactions here
            context.commit('reloadAccounts');
            return context.dispatch('getAccounts', { includeBalances: true, includeTransactions: true });
        })
        .catch((error) => alert(error));
    },
};

const mutations = {
    addAccount(accountState: any, account: Account) {
        const accountIndex = accountState.accounts.findIndex((acc: Account) => acc.id === account.id);
        if (accountIndex !== -1) {
            Vue.set(accountState.accounts, accountIndex, account);
        } else {
            accountState.accounts.push(account);
        }
    },
    updateAccountBalance(accountState: any, payload: any) {
        Vue.set(payload.account, 'balance', payload.balance);
    },
    updateAccountTransactions(accountState: any, payload: any) {
        Vue.set(payload.account, 'transactions', payload.transactions);
    },
    reloadAccounts(accountState: any) {
        accountState.loadedHeaders = false ;
        accountState.loadedBalances = false ;
        accountState.loadedTransactions = false ;
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
