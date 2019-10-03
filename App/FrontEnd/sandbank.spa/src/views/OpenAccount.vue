<template>
    <div>
        <el-container>
            <h2>Account Application</h2>
        </el-container>     
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
                <el-button type="primary" @click="openAccount()">Open account</el-button>
            </el-form-item>
        </el-form>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component
export default class Accounts extends Vue {

    private displayName: string = '';
    private accountType: string = 'TRANSACTION';

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

    private openAccount(): void {
        // yolo
    }
}
</script>

<style scoped>

* {
    text-align: initial;
}

</style>


