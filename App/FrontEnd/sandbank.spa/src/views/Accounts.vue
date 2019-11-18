<template>
    <div>
        <b-loading :is-full-page="true" :active="!loadedAccounts"></b-loading>
        <PageTitle title="Your Accounts"></PageTitle>
        <ul >
            <li v-for="(account, index) in accounts" v-bind:key="index">
                <div class="box">
                    <div class="columns is-mobile level">
                        <div class="column is-narrow">
                            <figure class="image is-96x96">
                                <img class="is-rounded" src="https://source.unsplash.com/random/100x100">
                            </figure>
                        </div>
                        <div class="column is-hidden-mobile">
                            <router-link :to="{ name: 'transactions', params: { accountId: account.id }}">{{ account.displayName }}</router-link>
                            <p>
                                <small>{{ account.accountNumber }}</small>
                            </p>
                        </div>
                        <div class="column is-hidden-mobile has-text-right">
                            <p>Balance {{ account.balance | asCurrency('NZD') }}</p>
                        </div>
                        <div class="column is-hidden-tablet">
                            <router-link :to="{ name: 'transactions', params: { accountId: account.id }}">{{ account.displayName }}</router-link>
                            <p>
                                <small>Balance {{ account.balance | asCurrency('NZD') }}</small>
                            </p>
                        </div>
                    </div>
                     <b-collapse class="card is-shadowless is-hidden-mobile" :open="false">
                    <div
                        slot="trigger" 
                        slot-scope="props"
                        class="card-header"
                        role="button">
                        <p class="card-header-title">
                            View recent transactions
                        </p>
                        <a class="card-header-icon">
                            <b-icon
                                :icon="props.open ? 'caret-down' : 'caret-up'">
                            </b-icon>
                        </a>
                    </div>
                    <div class="card-content is-paddingless">
                        <div class="content">
                              <b-table :data="account.transactions" striped>
                                <template slot-scope="props">
                                    <b-table-column field="amount" label="Amount">
                                        {{ props.row.amount | asCurrency('NZD') }}
                                    </b-table-column>
                                    <b-table-column field="date" label="Date" numeric>
                                        {{ props.row.transactionTimeUtc | asDate }}
                                    </b-table-column>
                                    <b-table-column field="description" label="Description">
                                        {{ props.row.description }}
                                    </b-table-column>
                                </template>

                                <template slot="empty">
                                    <section class="section">
                                        <div class="content has-text-grey has-text-centered">
                                            <p>Nothing here.</p>
                                        </div>
                                    </section>
                                </template>
                            </b-table>
                        </div>
                    </div>
                    </b-collapse>
                </div>
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
import { accountStore } from '@/store/store';
import { authStore } from '@/store/store';
import PageTitle from '@/components/PageTitle.vue';

@Component({
    components: {
        PageTitle,
    },
})
export default class Accounts extends Vue {

    private activeName: number[] = [];
    private isOpen: boolean[] = [];

    private get locale() {
        return this.$store.getters[`${authStore}/locale`];
    }

    private get accounts(): Account[] {
        return this.$store.getters[`${accountStore}/accounts`];
    }

    private get loadedHeaders(): boolean {
        return this.$store.getters[`${accountStore}/loadedHeaders`];
    }

    private get loadedAccounts(): boolean {
        return this.$store.getters[`${accountStore}/loadedHeaders`] && this.$store.getters[`${accountStore}/loadedBalances`];
    }

    private mounted() {
        this.$store.dispatch(`${accountStore}/getAccounts`, { includeBalances: true, includeTransactions: true } );
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

p {
    margin: 0;
}

li {
    margin-bottom: 30px;
}

li.el-timeline-item {
    margin-bottom: 0px;
    text-align: start;
}

div.el-card__body {
    padding: 0px;
    padding-left: 20px;
}

div.el-collapse {
    border: none;
}
</style>


