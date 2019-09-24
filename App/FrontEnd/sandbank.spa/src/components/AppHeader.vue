<template>
  <div class="headerWrapper">
    <el-header class="header" style="height: inherit; padding: 0px;">
      <div class="container">
        <el-menu
          id="nav"
          :default-active="activeIndex"
          class="el-menu-demo"
          mode="horizontal"
          @select="handleSelect"
        >
          <el-menu-item index="1">
            <h1>
              <router-link to="/">SandBank</router-link>
            </h1>
          </el-menu-item>
          <el-menu-item index="2">
            <router-link to="/accounts">Accounts</router-link>
          </el-menu-item>
          <el-menu-item index="3" style="float: right;">
            <router-link to="/register">Register</router-link>
          </el-menu-item>
          <el-menu-item index="4" style="float: right;">
            <router-link to="/login">Login</router-link>
          </el-menu-item>
        </el-menu>
      </div>
    </el-header>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component
export default class AppHeader extends Vue {
  private activeIndex: string = '1';

  private handleSelect(): void {
    // yolo
  }
}
</script>

<style>
.headerWrapper {
  width: 100%;
}

.container {
  width: 40%;
  margin: auto;
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
