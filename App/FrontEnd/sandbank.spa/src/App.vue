<template>
  <div id="app">
    <AppHeader/>
    <div class="main-content">
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
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

.main-content {
  height: 100%;
  margin-left: auto;
  margin-right: auto;
  width: 40%;
}
</style>
