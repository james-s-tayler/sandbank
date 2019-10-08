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
                <el-form>
                    <div>
                        <el-divider content-position="left">From</el-divider>
                        <el-form-item prop="fromAccount">
                            <el-select v-model="from">
                                <el-option
                                    v-for="account in accounts"
                                    :key="account.id"
                                    :selected="selectedIndex"
                                    :label="account.displayName"
                                    :value="account.displayName">
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </div>
                    <el-divider content-position="left">To</el-divider>
                    <el-divider content-position="left">Transfer details</el-divider>
                </el-form>
            </div>

            <div v-show="activeStep === reviewConfirm">
                <p>Step 2</p>
            </div>

            <div v-show="activeStep === done">
                <p>Step 3</p>
            </div>
        </el-container>

        <el-button v-show="activeStep === enterDetails" @click="setStep(reviewConfirm)">Review & confirm</el-button>
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
    private from: string = '';
    private selectedIndex: number = 0;

    private activeStep: number = this.enterDetails;

    private get accounts() {
        return this.$store.getters[`${accountStore}/accounts`];
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

</style>


