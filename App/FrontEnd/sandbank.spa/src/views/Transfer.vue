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

        <el-button v-show="activeStep === enterDetails" @click="setStep(reviewConfirm)">Review & confirm</el-button>
        <el-button v-show="activeStep === reviewConfirm" @click="setStep(done)">Confirm your transfer</el-button>
        <el-button v-show="activeStep === reviewConfirm" @click="setStep(enterDetails)">Change details</el-button>
        <el-button v-show="activeStep !== done">Cancel</el-button>
        <el-button v-show="activeStep === done" @click="finish()">Done</el-button>
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

    private activeStep: number = this.enterDetails;

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

</style>


