<template>
  <div id="app">
    <AppHeader/>
    <router-view/>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import AppHeader from '@/components/AppHeader.vue';

@Component({
  components: {
    AppHeader,
    },
})
export default class App extends Vue {

}
</script>

<style>

</style>
