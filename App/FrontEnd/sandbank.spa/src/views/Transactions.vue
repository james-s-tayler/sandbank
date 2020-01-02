<template>
    <div>
        <PageTitle :title="'Account: ' + account.displayName"></PageTitle>
        <div class="columns is-vcentered is-gapless">
            <div class="column">
                <p class="is-size-5">Balance: {{ account.balance | asCurrency('NZD') }}</p>
            </div>
            <div class="column is-hidden-mobile">
                <b-field label="Select a date range">
                    <b-datepicker
                        placeholder="Click to select"
                        v-model="range"
                        range>
                        <div class="buttons">
                            <button class="button is-info" @click="range = customRange(7)">
                                <span>7 Days</span>
                            </button>
                            <button class="button is-info" @click="range = customRange(30)">
                                <span>30 Days</span>
                            </button>
                            <button class="button is-info" @click="range = customRange(90)">
                                <span>90 Days</span>
                            </button>
                        </div>
                    </b-datepicker>
                </b-field>
            </div>
            <div class="column is-hidden-tablet">
                <!-- working around a bug in buefy where the datepicker is broken on mobile -->
                <div class="buttons">
                    <button class="button is-info" @click="range = customRange(7)">
                        <span>7 Days</span>
                    </button>
                    <button class="button is-info" @click="range = customRange(30)">
                        <span>30 Days</span>
                    </button>
                    <button class="button is-info" @click="range = customRange(90)">
                        <span>90 Days</span>
                    </button>
                </div>
            </div>
        </div>
        <b-table 
            :data="account.transactions" 
            striped 
            paginated
            :per-page="pageSize"
            :current-page.sync="currentPage">
            
            <template slot-scope="props">
                <b-table-column field="amount" label="Amount">
                    {{ props.row.amount | asCurrency('NZD') }}
                </b-table-column>
                <b-table-column field="description" label="Description">
                    {{ props.row.description }}
                </b-table-column>
                <b-table-column field="transactionTimeUtc" label="Date">
                    {{ props.row.transactionTimeUtc | asDate }}
                </b-table-column>
            </template>

            <template slot="empty">
                <section class="section">
                    <div class="content has-text-grey has-text-centered">
                        <p>No Transactions.</p>
                    </div>
                </section>
            </template>
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
import PageTitle from '@/components/PageTitle.vue';

@Component({components: {
    PageTitle,
}})
export default class Transactions extends Vue {

    private range: Date[] = [];
    private currentPage: number = 1;
    private pageSize: number = 15;

    private get locale() {
        return this.$store.getters[`${authStore}/locale`];
    }

    private get defaultRange() {
        return [this.defaultStartDate,
                this.defaultEndDate];
    }

    private get account(): Account {
        const accountId: number = Number(this.$route.params.accountId);
        return this.$store.getters[`${accountStore}/accounts`].find((acc: Account) => acc.id === accountId);
    }

    private customRange(days: number) {
        const end = new Date();
        const start = new Date();
        start.setTime(start.getTime() - 3600 * 1000 * 24 * days);
        return [start, end];
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
    private onRangeChanged(value: Date[], oldValue: Date[]) {
        const start = value[0].toISOString();
        const end = value[1].toISOString();

        const loadTransactionsRequest: LoadTransactionsRequest = {
            account: this.account,
            range: [start, end],
        };

        this.$store.dispatch(`${accountStore}/getTransactions`, loadTransactionsRequest);
    }

    private goBack(): void {
        this.$router.go(-1);
    }

    private created() {
        this.range = this.defaultRange;
    }

    private mounted() {
        this.$store.dispatch(`${accountStore}/getAccounts`, { includeMetadata: true, includeBalances: true, includeTransactions: true } );
    }
}
</script>

<style>

</style>