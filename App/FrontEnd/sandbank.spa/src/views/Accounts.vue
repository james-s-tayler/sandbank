<template>
    <div>
        <ul>
            <li v-for="(account, index) in this.accounts" v-bind:key="index">
                <router-link :to="{ name: 'transactions', params: { userId: userId, accountId: account.id }}">{{ account.accountNumber }} {{ account.displayName }}</router-link>
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
    private userId: string = '';

    constructor() {
        super();
    }

    private created() {

        this.userId = this.$route.params.userId;

        this.$http.get(`/user/${this.userId}/account`)
        .then((response: AxiosResponse) => {
            response.data.forEach((account: Account) => {
                this.accounts.unshift(account);
            });
        })
        .catch((error) => alert(error));
     }
}
</script>

<style>

</style>


