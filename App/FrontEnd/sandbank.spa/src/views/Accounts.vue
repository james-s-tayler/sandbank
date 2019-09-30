<template>
    <div>
        <el-container>
            <h2>Your Accounts</h2>
        </el-container>     
        <ul>
            <li v-for="(account, index) in this.$store.state.accounts" v-bind:key="index">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <div style="display: flex; justify-content: flex-start; align-items: center;">
                                <el-image :key="index"
                                    style="width: 100px; height: 100px; border-radius: 50%;"
                                    src="https://source.unsplash.com/random/100x100"
                                    fit="cover"></el-image>
                                <div style="display: flex; flex-direction: column; justify-content: flex-start; align-items: flex-start; padding: 10px;">
                                    <router-link :to="{ name: 'transactions', params: { accountId: account.id }}">{{ account.displayName }}</router-link>
                                    <p>
                                        <small>{{ account.accountNumber }}</small>
                                    </p>
                                </div>
                            </div>
                            <p>Balance ${{ account.balance }}</p>
                        </div>
                    </div>
                    <el-collapse v-model="activeName[index]" accordion>
                        <el-collapse-item title="View Recent Transactions" :name="account.id">
                            <el-timeline>
                                <el-timeline-item
                                    v-for="(transaction, transactionIndex) in account.transactions"
                                    :key="transactionIndex"
                                    :color="transaction.amount < 0 ? 'darkgray' : '#409EFF'"
                                    :timestamp="transaction.transactionTimeUtc">
                                    <strong>${{transaction.amount}}</strong> {{ transaction.description }}
                                </el-timeline-item>
                            </el-timeline>
                        </el-collapse-item>
                    </el-collapse>
                </el-card>
            </li>
        </ul>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Account } from '../account';
import { Route } from 'vue-router';
import Axios, { AxiosResponse } from 'axios';

@Component
export default class Accounts extends Vue {

    private activeName: number[] = [];

    constructor() {
        super();
    }

    private async created() {
        if (this.$store.state.accounts.length === 0) {

            const accounts: Account[] = await this.$http.get('/account')
            .then((response: AxiosResponse) => {
                return response.data;
            })
            .catch((error) => alert(error));

            accounts.forEach((account: Account) => {
                Axios.all(
                    [
                        this.$http.get(`/account/${account.id}/balance`),
                        this.$http.get(`/account/${account.id}/transaction`),
                    ],
                )
                .then(Axios.spread((balanceResponse: AxiosResponse, transactionResponse: AxiosResponse) => {
                    account.balance = balanceResponse.data;
                    account.transactions = transactionResponse.data;

                    this.$store.commit('addAccount', account);
                }))
                .catch((error) => {
                    account.balance = 0;
                    account.transactions = [];
                });
            });
        }
     }

     private goBack(): void {
         this.$router.go(-1);
     }
}
</script>

<style>
ul {
    list-style: none;
}

p {
    margin: 0;
}

li {
    margin-bottom: 30px;
}

li.el-timeline-item {
    margin-bottom: 0px;
    text-align: start;
}

div.el-card__body {
    padding: 0px;
    padding-left: 20px;
}

div.el-collapse {
    border: none;
}
</style>


