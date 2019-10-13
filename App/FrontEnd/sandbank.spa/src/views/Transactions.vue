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
                start-placeholder="Start date"
                end-placeholder="End date"
                :picker-options="pickerOptions">
            </el-date-picker>
        </el-container>
        <el-table :data="account.transactions" empty-text="No Transactions." stripe style="width: 100%">
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
import { Component, Vue, Watch } from 'vue-property-decorator';
import { Route } from 'vue-router';
import Axios, { AxiosResponse } from 'axios';
import { Transaction } from '../transaction';
import { Account } from '@/account';
import { accountStore } from '@/store/store';

@Component
export default class Transactions extends Vue {

        private range: string = '';

        private pickerOptions: any = {
          shortcuts: [{
            text: 'Last week',
            onClick(picker: any) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
              picker.$emit('pick', [start, end]);
            },
          }, {
            text: 'Last month',
            onClick(picker: any) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
              picker.$emit('pick', [start, end]);
            },
          }, {
            text: 'Last 3 months',
            onClick(picker: any) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
              picker.$emit('pick', [start, end]);
            },
          }],
        };

    private get account(): Account {
       const accountId: number = Number(this.$route.params.accountId);
       return this.$store.getters[`${accountStore}/accounts`].find((acc: Account) => acc.id === accountId);
    }

    @Watch('range')
    private onRangeChanged(value: string, oldValue: string) {
        // console.log(`oldRange: ${oldValue}, newRange: ${value}`);
    }

    private goBack(): void {
        this.$router.go(-1);
    }
}
</script>

<style>

</style>