<template>
  <div id="app">
    <div class="headerWrapper">
      <el-header class="header" style="height: inherit; padding: 0px;">
        <div class="container">
            <el-menu id="nav" :default-active="activeIndex" class="el-menu-demo" mode="horizontal" @select="handleSelect">
              <el-menu-item index="1">
                <h1>
                    <router-link to="/">SandBank</router-link>  
                </h1>
              </el-menu-item>
              <el-menu-item index="2">
                <router-link to="/accounts">Accounts</router-link>
              </el-menu-item>
              <el-menu-item index="3">
                <router-link to="/register">Register</router-link>
              </el-menu-item>
              <el-menu-item index="4">
                <router-link to="/login">Login</router-link>    
              </el-menu-item>
          </el-menu>
        </div>
      </el-header>
    </div>
    <div class="main-content">
      <router-view/>
    </div>
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

.headerWrapper {
  width: 100%;
}

.container {
  width: 40%; 
  margin: auto;
}

.main-content {
  height: 100%;
  
  margin-left: auto;
  margin-right: auto;
  width: 40%;
}

.header h1 {
    margin: 0;
    float: left;
    font-size: 32px;
    font-weight: 400;
}

.header h1 a {
  vertical-align: baseline;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
  text-decoration: none;
}

#nav a.router-link-exact-active {
  color: darkblue;
}
</style>
