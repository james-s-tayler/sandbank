<template>
  <div class="headerWrapper">
    <el-header class="header" style="height: inherit; padding: 0px;">
      <div class="container">
        <el-menu
          id="nav"
          :default-active="$route.path"
          class="el-menu-demo"
          mode="horizontal"
          :router="true"
        >
          <el-menu-item index="/">
            <h1>SandBank</h1>
          </el-menu-item>
          <el-menu-item v-if="isAuthenticated" index="/accounts">
            Accounts
          </el-menu-item>
          <el-menu-item v-if="isAuthenticated" index="/apply">
            Apply & open
          </el-menu-item>
          <el-menu-item v-if="isAuthenticated" index="/transfer">
            Transfer
          </el-menu-item>
          <el-menu-item v-if="!isAuthenticated" index="/login" style="float: right;">
            Login
          </el-menu-item>
          <el-menu-item v-if="!isAuthenticated" index="/register" style="float: right;">
            Register
          </el-menu-item>
          <el-menu-item v-if="isAuthenticated" @click="logout()" style="float: right;">
            Logout
          </el-menu-item>
        </el-menu>
      </div>
    </el-header>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { Route } from 'vue-router';
import { authStore } from '@/store/store';

@Component
export default class AppHeader extends Vue {

  public logout() {
    this.$store.dispatch(`${authStore}/logout`);
    this.$router.push('/');
  }

  private get isAuthenticated() {
    return this.$store.getters[`${authStore}/isAuthenticated`];
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
