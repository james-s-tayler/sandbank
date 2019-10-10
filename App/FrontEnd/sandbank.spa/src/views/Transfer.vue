<template>
    <div>
        <el-container>
            <h2>Transfer funds</h2>
        </el-container>     
        <el-steps :active="activeStep" finish-status="success">
            <el-step title="Enter Details" icon="el-icon-bank-card"></el-step>
            <el-step title="Review & Confirm" icon="el-icon-search"></el-step>
            <el-step title="Done" icon="el-icon-check"></el-step>
        </el-steps>
        <el-container class="transferContainer">
            <div v-show="activeStep === enterDetails">   
                <el-divider content-position="left">From</el-divider>
                <div>
                    <el-select v-model="fromAccountId" v-loading="!loadedAccounts">
                        <el-option 
                            disabled
                            :value="0" 
                            label="Select an account">
                        </el-option>
                        <el-option
                            v-for="account in accounts"
                            :key="account.id"
                            :label="account.displayName"
                            :value="account.id">
                            <span style="float: left">{{ account.displayName }}</span>
                            <span style="float: right; color: #8492a6; font-size: 13px">${{ account.balance }}</span>
                        </el-option>
                    </el-select>
                    <div v-if="fromAccount !== undefined" style="display: flex; font-size: smaller; padding-top: 10px;">
                        <div style="padding-right: 15px;">
                            <p>Account number</p>
                            <p>{{ fromAccount.accountNumber }}</p>
                        </div>
                        <div>
                            <p>Account balance</p>
                            <p>${{ fromAccount.balance }}</p>
                        </div>
                    </div>
                </div>

                <el-divider content-position="left">To</el-divider>
                <div>
                    <el-select v-model="toAccountId" v-loading="!loadedAccounts">
                        <el-option 
                            disabled
                            :value="0" 
                            label="Select an account">
                        </el-option>
                        <el-option
                            v-for="account in accounts"
                            :key="account.id"
                            :label="account.displayName"
                            :value="account.id">
                            <span style="float: left">{{ account.displayName }}</span>
                            <span style="float: right; color: #8492a6; font-size: 13px">${{ account.balance }}</span>
                        </el-option>
                    </el-select>
                    <div v-if="toAccount !== undefined" style="display: flex; font-size: smaller; padding-top: 10px;">
                        <div style="padding-right: 15px;">
                            <p>Account number</p>
                            <p>{{ toAccount.accountNumber }}</p>
                        </div>
                        <div>
                            <p>Account balance</p>
                            <p>${{ toAccount.balance }}</p>
                        </div>
                    </div>
                </div>

                <el-divider content-position="left">Transfer details</el-divider>
                <div style="width: 220px;">
                    <label for="amount">Amount</label>
                    <el-input 
                        v-model="amount"
                        :disabled="fromAccount === undefined || toAccount === undefined || fromAccount === toAccount"
                        name="amount"
                        type="number"
                        label="Amount"
                        :min="0"
                        :max="fromAccount === undefined ? 0 : fromAccount.balance">
                        <template slot="prepend">$</template>
                    </el-input>
                </div>
            </div>

            <div v-show="activeStep === reviewConfirm">
                <p>Step 2</p>
            </div>

            <div v-show="activeStep === done">
                <p>Step 3</p>
            </div>
        </el-container>

        <el-button v-show="activeStep === enterDetails" :disabled="!validTransfer" @click="setStep(reviewConfirm)">Review & confirm</el-button>
        <el-button v-show="activeStep === reviewConfirm" @click="setStep(done)">Confirm your transfer</el-button>
        <el-button v-show="activeStep === reviewConfirm" @click="setStep(enterDetails)">Change details</el-button>
        <el-button v-show="activeStep !== done" @click="dialogVisible = true">Cancel</el-button>
        <el-button v-show="activeStep === done" @click="finish">Done</el-button>

        <el-dialog
            title="You have unsaved changes"
            :visible.sync="dialogVisible"
            width="30%">
            <span>Clicking confirm will lose your unsaved changes. Are you sure you want to proceed?</span>
            <span slot="footer" class="dialog-footer">
                <el-button @click="dialogVisible = false">Cancel</el-button>
                <el-button type="primary" @click="finish">Confirm</el-button>
            </span>
        </el-dialog>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import Axios, { AxiosResponse } from 'axios';
import { Account } from '@/account';
import { accountStore } from '@/store/store';

@Component
export default class Transfer extends Vue {

    private enterDetails: number = 0;
    private reviewConfirm: number = 1;
    private done: number = 3;
    private dialogVisible: boolean = false;
    private fromAccountId: number = 0;
    private toAccountId: number = 0;
    private amount: number = 0;

    private activeStep: number = this.enterDetails;

    private get validTransfer(): boolean {
        return this.fromAccount !== undefined &&
               this.toAccount !== undefined &&
               this.fromAccount.id !== this.toAccount.id &&
               this.amount > 0 &&
               this.fromAccount.balance >= this.amount;
    }

    private get fromAccount(): Account {
        return this.accounts.find((acc: Account) => acc.id === this.fromAccountId);
    }

    private get toAccount(): Account {
        return this.accounts.find((acc: Account) => acc.id === this.toAccountId);
    }

    private get accounts() {
        return this.$store.getters[`${accountStore}/accounts`];
    }

    private get loadedAccounts(): boolean {
        return this.$store.getters[`${accountStore}/loadedHeaders`] && this.$store.getters[`${accountStore}/loadedBalances`];
    }

    private created() {
        this.$store.dispatch(`${accountStore}/getAccounts`, { includeBalances: true });
    }

    private setStep(step: number) {
        this.activeStep = step;
    }

    private finish() {
        this.$router.replace('/accounts');
    }
}
</script>

<style scoped>

* {
    text-align: initial;
}

.transferContainer {
    padding-top: 30px;
    padding-bottom: 30px;
}

.transferContainer > div {
    width: 100%;
}

.el-divider {
    background-color: #409EFF;
}

</style>


