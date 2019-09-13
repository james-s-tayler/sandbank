<template>
    <div>
        <ul>
            <li>userId: {{ userId }}</li>
            <li>accountId: {{ accountId }}</li>
            <li>balance: {{ balance }}</li>
        </ul>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { Route } from 'vue-router';
import Axios, { AxiosResponse } from 'axios';

@Component
export default class Transactions extends Vue {

    private userId: string = '';
    private accountId: string = '';
    private balance: number = 0;

    private created() {
       this.userId = this.$route.params.userId;
       this.accountId = this.$route.params.accountId;
       this.getBalance();
    }

    private getBalance() {

        this.$http.get('http://localhost:5100/api/user/' + this.userId + '/account/' + this.accountId + '/balance')
        .then((response: AxiosResponse) => this.balance = response.data)
        .catch((error) => 0);
    }
}
</script>

<style>

</style>