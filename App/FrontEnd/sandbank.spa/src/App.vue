<template>
  <div id="app">
    <el-container>
      <el-menu :default-active="activeIndex" class="el-menu-demo" mode="horizontal" @select="handleSelect">
        <el-menu-item index="1">SandBank</el-menu-item>
        <el-menu-item index="2">
          <router-link to="/">Register</router-link>
        </el-menu-item>
        <el-menu-item index="3">
          <router-link to="/login">Login</router-link>    
        </el-menu-item>
        <el-menu-item index="4">
          <router-link to="/accounts">Accounts</router-link>
        </el-menu-item>
      </el-menu>
    </el-container>
    
    <router-view/>
  </div>
</template>

<script lang="ts">

import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component
export default class Accounts extends Vue {

    private activeIndex: string = '1';

    private handleSelect(): void {
      // yolo
    }

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
#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}
</style>
