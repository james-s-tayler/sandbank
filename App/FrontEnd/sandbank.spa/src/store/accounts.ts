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
    loadedMetadata: false ,
    accounts: Array< Account>(),
};

const getters = {
    accounts: (accountState: any) => {
        return accountState.accounts;
    },
    loadedHeaders: (accountState: any) => {
        return accountState.loadedHeaders;
    },
    loadedMetadata: (accountState: any) => {
        return accountState.loadedMetadata;
    },
    loadedBalances: (accountState: any) => {
        return accountState.loadedBalances;
    },
    loadedAccounts: (accountState: any) => {
        return accountState.loadedHeaders && accountState.loadedBalances && accountState.loadedMetadata;
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

        if (!context.state.loadedMetadata && payload.includeMetadata) {

            const getMetadataPromises = new Array< Promise< AxiosResponse< any>>>();
            const accountMap = new Map();

            context.state.accounts.forEach((account: Account) => {
                const getMetadataUrl = `/account/${account.id}/metadata`;
                accountMap.set(getMetadataUrl, account);
                getMetadataPromises.push(Axios.get(getMetadataUrl));
            });

            Axios.all(getMetadataPromises).then((responses: Array< AxiosResponse< any>>) => {
                responses.forEach((response: AxiosResponse) => {
                    const url = (response.config.url || '').replace(response.config.baseURL as string, '');

                    const account = accountMap.get(url);
                    const metadata = response.data;

                    context.commit('updateAccountMetadata', { account, metadata });
                });
            })
            //I guess this should fall back to AccountType + Placeholder image url?
            //.catch((error) => {
            //    accountMap.forEach((key: string, value: Account) => {
            //        context.commit('updateAccountBalance',  { value, balance: 0 });
            //    });
            //})
            .finally(() => {
                context.commit('finishedLoadingMetadata');
            });
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
            context.commit('reloadAccounts', { reloadBalances: true, reloadTransactions: true });
            return context.dispatch('getAccounts', { includeBalances: true, includeTransactions: true });
        })
        .catch((error) => alert(error));
    },
    async updateMetadata(context: ActionContext< any, any>, accountMetadata: any) {
        Axios.post(`/account/${accountMetadata.accountId}/metadata`, accountMetadata).then(() => {
            // should modify this method just to refresh a single account / balance / transactions here
            context.commit('reloadAccounts', { reloadMetadata: true });
            return context.dispatch('getAccounts', { includeMetadata: true });
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
    updateAccountMetadata(accountState: any, payload: any) {
        Vue.set(payload.account, 'displayName', payload.metadata.nickname);
        Vue.set(payload.account, 'imageUrl', payload.metadata.imageUrl);
    },
    updateAccountBalance(accountState: any, payload: any) {
        Vue.set(payload.account, 'balance', payload.balance);
    },
    updateAccountTransactions(accountState: any, payload: any) {
        Vue.set(payload.account, 'transactions', payload.transactions);
    },
    reloadAccounts(accountState: any, payload: any) {
        if (payload.reloadHeaders) {
            accountState.loadedHeaders = false ;    
        }
        if (payload.reloadTransactions) {
            accountState.loadedTransactions = false ;
        }
        if (payload.reloadMetadata) {
            accountState.loadedMetadata = false ;
        }
        if(payload.reloadBalances) {
            accountState.loadedBalances = false ;
        }
    },
    finishedLoadingHeaders(accountState: any) {
        accountState.loadedHeaders = true;
    },
    finishedLoadingMetadata(accountState: any) {
        accountState.loadedMetadata = true;
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
