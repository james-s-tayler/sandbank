<template>
    <div>
        
        <el-page-header style="padding-top: 20px;" @back="goBack" :content="'Balance: $' + balance" title="Back"></el-page-header>
        <el-container>
            <h2>Transactions</h2>
        </el-container>
        <el-table :data="transactions" stripe style="width: 100%">
            <el-table-column
                prop="description"
                label="Description">
            </el-table-column>
            <el-table-column
                prop="amount"
                label="Amount">
            </el-table-column>
        </el-table>
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
        this.$http.get(`/account/${this.accountId}/balance`)
        .then((response: AxiosResponse) => this.balance = response.data)
        .catch((error) => this.balance = 0);
    }

    private getTransactions() {
        this.$http.get(`/account/${this.accountId}/transaction`)
        .then((response: AxiosResponse) => {
            response.data.forEach((txn: Transaction) => {
               this.transactions.unshift(txn);
            });
        })
        .catch((error) => alert(error));
    }

    private goBack(): void {
        this.$router.go(-1);
    }
}
</script>

<style>

</style>