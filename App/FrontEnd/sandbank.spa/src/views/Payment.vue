<template>
    <div>
        <PageTitle title="Pay a person or a bill"></PageTitle>     
        <div class="box">
            <b-steps v-model="activeStep" type="is-info" size="is-medium" :has-navigation="false">
                <b-step-item label="Enter Details" icon-pack="fas" icon="money-check-alt"></b-step-item>
                <b-step-item label="Review & Confirm" icon-pack="fas" icon="search"></b-step-item>
                <b-step-item label="Done" icon-pack="fas" icon="check-circle"></b-step-item>
            </b-steps>
            <div v-show="activeStep === enterDetails">
                <div class="columns is-gapless">
                    <div class="column is-3">
                        <b-field label="From">
                            <b-select v-model="fromAccountId">
                                <option :value="0" disabled>
                                    Select an account
                                </option>
                                <option v-for="account in accounts" 
                                    :key="account.id"
                                    :value="account.id">
                                    <div class="level">
                                        <div class="level-left">
                                            <div class="level-item">
                                                {{ account.displayName }}
                                            </div>
                                        </div>
                                        <div class="level-right">
                                            <div class="level-item">
                                                {{ account.balance | asCurrency('NZD') }}
                                            </div>
                                        </div>
                                    </div>
                                </option>
                            </b-select>
                        </b-field>

                        <b-field label="To">
                            <b-input
                                ref="accountInput"
                                v-model="toAccountNumber" 
                                pattern="[0-9]{2}-[0-9]{4}-[0-9]{7}-[0-9]{2}"
                                validation-message="Please match the format: 00-0000-0000000-00"
                                @keyup.native="$refs['accountInput'].checkHtml5Validity()"
                                placeholder="00-0000-0000000-00"></b-input>
                        </b-field>
                        <b-field label="Amount">
                            <b-input
                                v-model="amount"
                                :disabled="!validToAccount"
                                is-numeric
                                :min="0"
                                icon-pack="fas"
                                icon="dollar-sign"
                                :max="fromAccount === undefined ? 0 : fromAccount.balance">
                            </b-input>
                        </b-field>
                        <b-field label="Reference">
                            <b-input
                                v-model="reference"
                                :disabled="!validToAccount">
                            </b-input>
                        </b-field>
                        <b-field>
                            <div class="buttons">
                                <b-button v-show="activeStep === enterDetails" type="is-info" :disabled="!validTransfer" @click="setStep(reviewConfirm)">Review & confirm</b-button>
                                <b-button v-show="activeStep !== done" type="is-info" @click="confirmCancel">Cancel</b-button>
                            </div>
                        </b-field>
                    </div>
                </div>
            </div>

            <div v-if="activeStep === reviewConfirm && fromAccount !== undefined">
                <div class="columns is-vcentered">
                    <div class="column">
                        <div class="box">
                            <p class="title is-marginless"> {{ fromAccount.displayName }}</p>
                            <p class="subtitle is-marginless"> {{ fromAccount.accountNumber }}</p>
                            <p class="is-size-6">After: {{ fromAccount.balance - amount | asCurrency('NZD') }}</p>
                        </div>
                    </div>
                    <div class="column">
                        <p class="title has-text-centered is-marginless">Transfer</p>
                        <p v-if="reference" class="is-size-6 has-text-centered">{{ reference }}</p>
                        <p class="subtitle has-text-centered is-marginless">{{ amount | asCurrency('NZD') }}</p>
                        
                    </div>
                    <div class="column">
                        <div class="box">
                            <p class="title is-marginless"> Pay To</p>
                            <p class="subtitle is-marginless"> {{ toAccountNumber }}</p>
                            <p class="is-size-6">On: today</p>
                        </div>
                    </div>
                </div>
                <div class="buttons">
                    <b-button v-show="activeStep === reviewConfirm" type="is-info" @click="confirmTransfer">Confirm transfer</b-button>
                    <b-button v-show="activeStep === reviewConfirm" type="is-info" @click="setStep(enterDetails)">Change</b-button>
                    <b-button v-show="activeStep !== done" type="is-info" @click="confirmCancel">Cancel</b-button>
                </div>
            </div>

            <div v-show="activeStep === done">
                <b-notification
                    type="is-success"
                    has-icon
                    :closable="false">
                    Your transfer of {{ amount | asCurrency('NZD') }} has been made.
                </b-notification>
                <div class="buttons">
                    <b-button v-show="activeStep === done" type="is-info" @click="finish">Done</b-button>
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
import { PostPaymentRequest } from '@/models/requests/post-payment-request';
import { Position } from 'vue-router/types/router';
import PageTitle from '@/components/PageTitle.vue';

@Component({
    components: {
        PageTitle,
    },
})
export default class Transfer extends Vue {

    private enterDetails: number = 0;
    private reviewConfirm: number = 1;
    private done: number = 2;
    private dialogVisible: boolean = false;
    private fromAccountId: number = 0;
    private toAccountNumber: string = '';
    private amount: number = 0;
    private reference: string = '';

    private activeStep: number = this.enterDetails;

    private get validToAccount(): boolean {
        return this.fromAccount !== undefined &&
               this.toAccountNumber !== '' &&
               this.fromAccount.accountNumber !== this.toAccountNumber &&
               (this.$refs['accountInput'] as any).isValid; //fix this with proper type
    }

    private get validAmount(): boolean {
        return this.amount > 0 &&
               this.fromAccount.balance >= this.amount;
    }

    private get validTransfer(): boolean {
        return this.validToAccount &&
               this.validAmount;
    }

    private get fromAccount(): Account {
        return this.accounts.find((acc: Account) => acc.id === this.fromAccountId);
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

    private confirmTransfer() {

        const paymentRequest: PostPaymentRequest = {
            fromAccount: this.fromAccount.accountNumber,
            toAccount: this.toAccountNumber,
            amount: this.amount,
            description: this.reference !== '' ? this.reference : `Transfer to ${this.toAccountNumber}`,
            merchantName: 'payment',
        };

        this.$store.dispatch(`${accountStore}/transfer`, paymentRequest)
        .then((response: any) => {
            this.setStep(this.done);
        });
    }

    private confirmCancel() {
        this.$buefy.dialog.confirm({
                    title: 'You have unsaved changes',
                    message: 'Clicking confirm will lose your unsaved changes. Are you sure you want to proceed?',
                    confirmText: 'Confirm',
                    type: 'is-danger',
                    hasIcon: true,
                    onConfirm: () => this.finish(),
                });
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

</style>


