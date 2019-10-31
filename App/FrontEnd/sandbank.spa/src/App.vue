<template>
  <div id="app">
    <AppHeader/>
    <div class="container is-fullhd">
      <router-view/>
    </div>
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
