<template>
    <div>
        <PageTitle title="Account Application"></PageTitle>
        <div class="box">
        <div class="columns is-gapless">
            <div class="column">
                <section>
                    <b-field label="Account Type">
                        <b-select 
                            v-model="accountType"
                            placeholder="Select an account type"
                            required>
                            <option 
                                v-for="(type, index) in accountTypes"
                                :key="index"
                                :value="type.value">
                                {{ type.name }}
                            </option>
                        </b-select>
                    </b-field>
                    <b-field label="Account Name">
                        <b-input v-model="displayName" required></b-input>
                    </b-field>
                    <b-field>
                        <b-checkbox v-model="seedData">Seed dummy transaction data</b-checkbox>
                    </b-field>
                    <b-button @click="openAccount" type="is-info" :loading="processing">Open account</b-button>
                </section>
            </div>
            <div class="column">
            </div>
            <div class="column">
            </div>
        </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import Axios, { AxiosResponse } from 'axios';
import { Account } from '@/account';
import { accountStore } from '@/store/store';
import PageTitle from '@/components/PageTitle.vue';

@Component({
    components: {
        PageTitle,
    },
})
export default class Accounts extends Vue {

    private displayName: string = '';
    private accountType: string = 'TRANSACTION';
    private seedData: boolean = true;
    private processing: boolean = false;

    private get accountTypes() {
        return [
            {
                value: 'TRANSACTION',
                name: 'Everyday Account',
            },
            {
                value: 'SAVING',
                name: 'Super Saver Account',
            },
        ];
    }

    private async openAccount() {

        this.processing = true;

        const openAccountRequest = {
            accountType: this.accountType,
            displayName: this.displayName,
        };

        const headers = {
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
            },
        };

        const account = await this.$http.post('/account', openAccountRequest, headers)
        .then((response: AxiosResponse) => {
            return response.data;
        })
        .catch((error) => {
            alert(error);
            return -1;
        });

        if (account.id > -1 && this.seedData) {
            this.$http.post(`/account/${account.id}/seed`, {} , headers)
            .then((response: AxiosResponse) => {
                this.addAccountToStore(account);
            })
            .catch((error) => alert(error));
        } else {
            this.addAccountToStore(account);
        }
    }

    private addAccountToStore(account: Account) {
        Axios.all(
                    [
                        this.$http.get(`/account/${account.id}/balance`),
                        this.$http.get(`/account/${account.id}/transaction`),
                    ],
                )
                .then(Axios.spread((balanceResponse: AxiosResponse, transactionResponse: AxiosResponse) => {
                    account.balance = balanceResponse.data;
                    account.transactions = transactionResponse.data;

                    this.$store.commit(`${accountStore}/addAccount`, account);

                    this.$router.push('/accounts');
                }))
                .catch((error) => {
                    account.balance = 0;
                    account.transactions = [];
                });
    }
}
</script>

<style scoped>

</style>


