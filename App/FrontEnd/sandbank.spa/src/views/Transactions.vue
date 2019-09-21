<template>
    <div>
        <p>balance: {{ balance }}</p>
        <ul>
            <li v-for="(transaction, index) in this.transactions" v-bind:key="index"> ${{ transaction.amount }} {{ transaction.description }}</li>
        </ul>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { Route } from 'vue-router';
import Axios, { AxiosResponse } from 'axios';
import { Transaction } from '../transaction';

@Component
export default class Transactions extends Vue {

    private userId: string = '';
    private accountId: string = '';
    private balance: number = 0;
    private transactions: Transaction[] = [];

    private created() {
       this.accountId = this.$route.params.accountId;
       this.getBalance();
       this.getTransactions();
    }

    private getBalance() {
        this.$http.get(`/user/account/${this.accountId}/balance`)
        .then((response: AxiosResponse) => this.balance = response.data)
        .catch((error) => this.balance = 0);
    }

    private getTransactions() {
        this.$http.get(`/user/account/${this.accountId}/transaction`)
        .then((response: AxiosResponse) => {
            response.data.forEach((txn: Transaction) => {
               this.transactions.unshift(txn);
            });
        })
        .catch((error) => alert(error));
    }
}
</script>

<style>

</style>