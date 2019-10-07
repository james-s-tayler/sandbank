import { Account } from '@/account';
import Axios, { AxiosResponse } from 'axios';
import { ActionContext } from 'vuex';

const state = {
    loaded: false ,
    accounts: Array< Account>(),
};

const getters = {
    accounts: (accountState: any) => {
        return accountState.accounts;
    },
};

const actions = {
    async getAccounts(context: ActionContext< any, any>) {
        if (!context.state.loaded) {

            const accounts: Account[] = await Axios.get('/account')
            .then((response: AxiosResponse) => {
                return response.data;
            })
            .catch((error) => alert(error));

            accounts.forEach((account: Account) => {
                Axios.all(
                    [
                        Axios.get(`/account/${account.id}/balance`),
                        Axios.get(`/account/${account.id}/transaction`),
                    ],
                )
                .then(Axios.spread((balanceResponse: AxiosResponse, transactionResponse: AxiosResponse) => {
                    account.balance = balanceResponse.data;
                    account.transactions = transactionResponse.data;

                    context.commit('addAccount', account);
                }))
                .catch((error) => {
                    account.balance = 0;
                    account.transactions = [];
                });
            });

            context.commit('finishedLoading');
        }
    },
};

const mutations = {
    addAccount(accountState: any, account: Account) {
        accountState.accounts.push(account);
    },
    finishedLoading(accountState: any) {
        accountState.loaded = true;
    },
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations,
};
