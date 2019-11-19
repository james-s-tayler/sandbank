<template>
  <b-navbar 
    type="is-info" 
    wrapper-class="container" 
    spaced>
        <template slot="brand">
            <b-navbar-item tag="router-link" :to="{ path: '/' }">
                <h1>Sandbank</h1>
            </b-navbar-item>
        </template>
        <template v-if="isAuthenticated" slot="start">
            <b-navbar-item tag="router-link" to="/accounts">
                Accounts
            </b-navbar-item>
            <b-navbar-item tag="router-link" to="/apply">
                Apply & open
            </b-navbar-item>
            <b-navbar-item tag="router-link" to="/transfer">
                Transfer funds
            </b-navbar-item>
            <b-navbar-item tag="router-link" to="/payment">
                Pay a person or a bill
            </b-navbar-item>
            
        </template>

        <template slot="end">
            <b-navbar-item tag="div">
                <div class="buttons">
                    <router-link 
                      v-if="!isAuthenticated"
                      class="button is-light" 
                      to="/register">
                        <strong>Sign up</strong>
                    </router-link>
                    <router-link
                      v-if="!isAuthenticated" 
                      class="button is-primary" 
                      to="/login">
                        Log in
                    </router-link>
                    <a
                      v-if="isAuthenticated" 
                      class="button is-primary" 
                      @click="logout()">
                        Log out
                    </a>
                </div>
            </b-navbar-item>
        </template>
    </b-navbar>
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

</style>
