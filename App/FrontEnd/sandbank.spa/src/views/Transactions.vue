<template>
    <div>
        
        <el-page-header style="padding-top: 20px;" @back="goBack" :content="'Balance: $' + account.balance" title="Back"></el-page-header>
        <el-container>
            <h2>Transactions</h2>
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
import { Component, Vue } from 'vue-property-decorator';
import { Route } from 'vue-router';
import Axios, { AxiosResponse } from 'axios';
import { Transaction } from '../transaction';
import { Account } from '@/account';
import { accountStore } from '@/store/store';

@Component
export default class Transactions extends Vue {

    private get account(): Account {
       const accountId: number = Number(this.$route.params.accountId);
       return this.$store.getters[`${accountStore}/accounts`].find((acc: Account) => acc.id === accountId);
    }

    private goBack(): void {
        this.$router.go(-1);
    }
}
</script>

<style>

</style>