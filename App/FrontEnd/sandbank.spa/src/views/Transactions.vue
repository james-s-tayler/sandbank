<template>
    <div>
        
        <el-page-header style="padding-top: 20px;" @back="goBack" :content="'Balance: $' + account.balance" title="Back"></el-page-header>
        <el-container style="display: flex; justify-content: space-between; align-items: center;">
            <h2>Transactions</h2>
            <el-date-picker
                v-model="range"
                type="daterange"
                align="right"
                unlink-panels
                range-separator="To"
                format="dd/MM/yyyy"
                value-format="yyyy-MM-dd"
                :start-placeholder="defaultStartDate.toLocaleDateString(locale)"
                :end-placeholder="defaultEndDate.toLocaleDateString(locale)"
                :default-value="defaultRange"
                :picker-options="pickerOptions">
            </el-date-picker>
        </el-container>
        <el-table :data="transactionsPage" empty-text="No Transactions." stripe style="width: 100%">
            <el-table-column
                prop="amount"
                width="150px"
                label="Amount">
            </el-table-column>
            <el-table-column
                prop="description"
                label="Description">
            </el-table-column>
            <el-table-column
                prop="transactionTimeUtc"
                label="Date"
                width="250px">
            </el-table-column>
        </el-table>
        <el-pagination
            background
            layout="prev, pager, next"
            :current-page.sync="currentPage"
            :total="account.transactions.length">
        </el-pagination>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from 'vue-property-decorator';
import { Route } from 'vue-router';
import Axios, { AxiosResponse } from 'axios';
import { Transaction } from '../transaction';
import { Account } from '@/account';
import { accountStore } from '@/store/store';
import { LoadTransactionsRequest } from '@/models/requests/load-transactions-request';

@Component
export default class Transactions extends Vue {

        private range: string[] = [];
        private locale: string = 'en-NZ';
        private currentPage: number = 1;

        private pickerOptions: any = {
          shortcuts: [{
            text: 'Last 7 days',
            onClick(picker: any) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
              picker.$emit('pick', [start, end]);
            },
          }, {
            text: 'Last 30 days',
            onClick(picker: any) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
              picker.$emit('pick', [start, end]);
            },
          }, {
            text: 'Last 90 days',
            onClick(picker: any) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
              picker.$emit('pick', [start, end]);
            },
          }],
        };

    private get transactionsPage() {
        const pageSize = 10;
        return this.account.transactions.slice((this.currentPage - 1) * pageSize, this.currentPage * pageSize);
    }

    private get account(): Account {
       const accountId: number = Number(this.$route.params.accountId);
       return this.$store.getters[`${accountStore}/accounts`].find((acc: Account) => acc.id === accountId);
    }

    private get defaultRange() {
        return [this.defaultStartDate.toISOString().substring(0, 10),
                this.defaultEndDate.toISOString().substring(0, 10)];
    }

    private get defaultStartDate() {
        const start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
        return start;
    }

    private get defaultEndDate() {
        return new Date();
    }

    @Watch('range')
    private onRangeChanged(value: string[], oldValue: string[]) {
        const start = value[0];
        const end = value[1];

        const loadTransactionsRequest: LoadTransactionsRequest = {
            account: this.account,
            range: [start, end],
        };

        this.$store.dispatch(`${accountStore}/getTransactions`, loadTransactionsRequest);
    }

    private goBack(): void {
        this.$router.go(-1);
    }
}
</script>

<style>

</style>