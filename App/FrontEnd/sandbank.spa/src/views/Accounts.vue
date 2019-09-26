<template>
    <div>
        <el-container>
            <h2>Your Accounts</h2>
        </el-container>     
        <ul>
            <li v-for="(account, index) in this.accounts" v-bind:key="index">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <router-link :to="{ name: 'transactions', params: { accountId: account.id }}">{{ account.displayName }}</router-link>
                        <span><small> {{ account.accountNumber }}</small></span>
                        <el-button style="float: right; padding: 3px 0" type="text">Balance $0.00</el-button>
                    </div>
                    <el-collapse v-model="activeName[index]" accordion>
                        <el-collapse-item title="Recent Transactions" :name="account.id">
                            <div>Consistent with real life: in line with the process and logic of real life, and comply with languages and habits that the users are used to;</div>
                            <div>Consistent within interface: all elements should be consistent, such as: design style, icons and texts, position of elements, etc.</div>
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

    private accounts: Account[] = [];
    private activeName: number[] = [];

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


