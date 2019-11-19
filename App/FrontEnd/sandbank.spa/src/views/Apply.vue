<template>
    <div>
        <PageTitle title="Apply & open"></PageTitle>
        <ul>
            <li v-for="(option, index) in options" v-bind:key="index">
                <div class="box">
                <div class="columns is-vcentered">
                    <div class="column is-narrow is-hidden-mobile">
                        <b-icon 
                            icon="money-bill" 
                            size="is-large"
                            type="is-info">
                        </b-icon>
                    </div>
                    <div class="column">
                        <h2 class="subtitle is-marginless">
                            <strong>{{option.type}}</strong>
                        </h2>
                        <p>{{option.description}}</p>
                    </div>
                    <div class="column">
                        <router-link :to="{ name: option.linkTo }">
                            <b-button type="is-info">
                                {{ option.callToAction }}
                            </b-button>
                        </router-link>
                    </div>
                    <div class="column">
                    </div>
                </div>
                </div>
            </li>
        </ul>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import PageTitle from '@/components/PageTitle.vue';

@Component({
    components: {
        PageTitle,
    },
})
export default class Accounts extends Vue {

    private get options() {
        return [
            {
                icon: 'money',
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


