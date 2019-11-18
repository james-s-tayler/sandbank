<template>
    <div class="hero is-small is-hidden-mobile">
        <div class="hero-body is-paddingless-left">
            <div class="container">
                <h1 v-if="title" class="title">{{ title }}</h1>
                <h2 v-if="subtitle" class="subtitle">{{ subtitle }}</h2>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';

@Component
export default class PageTitle extends Vue {

    @Prop({required: true})
    public title!: string;

    @Prop()
    public subtitle?: string;

}
</script>

<style scoped>
.is-paddingless-left {
    padding-left: 0;
}
</style>