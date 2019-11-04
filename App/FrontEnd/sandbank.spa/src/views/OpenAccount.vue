<template>
    <div>
        <PageTitle title="Account Application"></PageTitle>
        <el-form status-icon label-width="120px">
            <el-form-item label="Account Type" prop="accountType">
                <el-select v-model="accountType">
                    <el-option
                        v-for="type in accountTypes"
                        :key="type.value"
                        :label="type.name"
                        :value="type.value">
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="Account Name" prop="displayName">
                <el-input type="text" v-model="displayName" required></el-input>
            </el-form-item>
            <el-form-item>
                 <el-checkbox v-model="seedData">Seed dummy transaction data</el-checkbox>
            </el-form-item>
            <el-form-item>
                <el-button v-loading="processing" type="primary" @click="openAccount()">Open account</el-button>
            </el-form-item>
        </el-form>
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

* {
    text-align: initial;
}

</style>


