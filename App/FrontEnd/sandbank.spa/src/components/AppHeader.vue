<template>
  <div class="headerWrapper">
    <el-header class="header" style="height: inherit; padding: 0px;">
      <div class="container">
        <el-menu
          id="nav"
          :default-active="activeIndex"
          class="el-menu-demo"
          mode="horizontal"
        >
          <el-menu-item index="1">
            <h1>
              <router-link to="/">SandBank</router-link>
            </h1>
          </el-menu-item>
          <el-menu-item v-if="isAuthenticated" index="2">
            <router-link to="/accounts">Accounts</router-link>
          </el-menu-item>
          <el-menu-item v-if="!isAuthenticated" index="3" style="float: right;">
            <router-link to="/login">Login</router-link>
          </el-menu-item>
          <el-menu-item v-if="!isAuthenticated" index="4" style="float: right;">
            <router-link to="/register">Register</router-link>
          </el-menu-item>
          <el-menu-item v-if="isAuthenticated" index="4" style="float: right;">
            <a @click="logout()" href="#">Logout</a>
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

@Component
export default class AppHeader extends Vue {

  private activeIndex: string = '1';

  public logout() {
    this.$store.dispatch('logout');
    this.$router.push('/');
  }

  private get isAuthenticated() {
    return this.$store.getters.isAuthenticated;
  }

  private created() {
    const expiration = window.sessionStorage.getItem('authTokenExpiration');
    const unixTimestamp = new Date().getUTCMilliseconds() / 1000;

    if (expiration !== null && parseInt(expiration, 10) - unixTimestamp > 0) {
      this.$store.commit('updateAuthStatus', true);
    } else {
      this.logout();
    }
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
