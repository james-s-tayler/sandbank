<template>
    <div>
        <el-container>
            <h2>Apply & open</h2>
        </el-container>     
        <ul>
            <li v-for="(option, index) in options" v-bind:key="index">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <div style="display: flex; justify-content: space-between; align-items: center;">
                            <div style="display: flex; justify-content: flex-start; align-items: center;">
                                <i :class="option.icon"></i>
                                <div style="display: flex; flex-direction: column; justify-content: flex-start; align-items: flex-start; padding-left: 10px;">
                                    <h3>{{ option.type }}</h3>
                                    <p>{{ option.description }}</p>
                                </div>
                            </div>
                            <router-link :to="{ name: option.linkTo }">
                                <el-button type="primary">
                                    {{ option.callToAction }}
                                </el-button>
                            </router-link>
                        </div>
                    </div>
                </el-card>
            </li>
        </ul>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';

@Component
export default class Accounts extends Vue {

    private get options() {
        return [
            {
                icon: 'el-icon-money',
                type: 'Bank account',
                description: 'Manage your money the way you want.',
                callToAction: 'Open a bank account',
                linkTo: 'openAccount',
            },
        ];
    }
}
</script>

<style scoped>
ul {
    list-style: none;
}

p {
    margin: 0;
}

i {
    font-size: 42px;
    height: 42px;
    color: deepskyblue;
}

h3 {
    margin-bottom: 7px;
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

a {
    color: white;
    text-decoration: none;
}
</style>


