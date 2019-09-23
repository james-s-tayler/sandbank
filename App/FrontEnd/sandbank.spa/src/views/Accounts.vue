<template>
    <div>
        <el-container>
            <h2>Your Accounts</h2>
        </el-container>
        
        <ul>
            <li v-for="(account, index) in this.accounts" v-bind:key="index">
                <router-link :to="{ name: 'transactions', params: { accountId: account.id }}">{{ account.accountNumber }} {{ account.displayName }}</router-link>
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

    private accounts: Account[] = [];

    constructor() {
        super();
    }

    private created() {
        this.$http.get('/account')
        .then((response: AxiosResponse) => {
            response.data.forEach((account: Account) => {
                this.accounts.unshift(account);
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
ul {
    list-style: none;
}
</style>


