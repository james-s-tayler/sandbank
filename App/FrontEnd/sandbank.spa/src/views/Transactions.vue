<template>
    <div>
        
        <el-page-header
            style="padding-top: 20px;" 
            @back="goBack" 
            :content="account.balance | asCurrency('NZD') | prepend('Balance: ')" 
            title="Back"></el-page-header>
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
                :start-placeholder="defaultStartDate | asDate"
                :end-placeholder="defaultEndDate | asDate"
                :default-value="defaultRange"
                :picker-options="pickerOptions">
            </el-date-picker>
        </el-container>
        <b-table :data="transactionsPage" striped>
            <template slot-scope="props">
                <b-table-column field="amount" label="Amount">
                    {{ props.row.amount | asCurrency('NZD') }}
                </b-table-column>
                <b-table-column field="description" label="Description">
                    {{ props.row.description }}
                </b-table-column>
                <b-table-column field="transactionTimeUtc" label="Date">
                    {{ props.row.transactionTimeUtc }}
                </b-table-column>
            </template>
            <!-- add empty slot too -->
        </b-table>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from 'vue-property-decorator';
import { Route } from 'vue-router';
import Axios, { AxiosResponse } from 'axios';
import { Transaction } from '../transaction';
import { Account } from '@/account';
import { accountStore } from '@/store/store';
import { authStore } from '@/store/store';
import { LoadTransactionsRequest } from '@/models/requests/load-transactions-request';

@Component
export default class Transactions extends Vue {

        private range: string[] = [];
        private currentPage: number = 1;

        private moneyFormatter: Intl.NumberFormat = new Intl.NumberFormat(this.locale, {
            style: 'currency',
            currency: 'NZD',
        });

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

    private get locale() {
        return this.$store.getters[`${authStore}/locale`];
    }

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

    // would be nice to be able to re-use the proper i18n filters in main.ts
    private formatDate(row: number, column: number , cellValue: string, index: number) {
        return new Date(cellValue).toLocaleString(this.locale);
    }

    private formatMoney(row: number, column: number , cellValue: string, index: number) {
        return this.moneyFormatter.format(Number(cellValue));
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