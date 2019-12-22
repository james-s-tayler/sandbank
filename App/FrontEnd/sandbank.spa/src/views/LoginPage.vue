<template>
  <div class="columns is-mobile is-flex is-centered is-vcentered is-gapless is-full-height">
    <div class="column is-one-quarter-desktop is-three-quarters-mobile">
      <b-field label="Email">
        <b-input 
          type="email"
          placeholder="you@sandbank.com"
          v-model="email"
          icon-pack="fas"
          icon="envelope"
          @keyup.native.enter="login">
        </b-input>
      </b-field>
      <a class="button is-primary" @click="login">Log on</a>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { AxiosResponse } from 'axios';
import { authStore } from '@/store/store';

@Component
export default class LoginPage extends Vue {
   private email: string = '';

  private login() {
    const loginUserRequest = {
      email: this.email,
      password: 'a',
    };

    const headers = {
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    };

    this.$http
      .post('/user/login', loginUserRequest, headers)
      .then((response: AxiosResponse) => {
        this.$store.dispatch(`${authStore}/login`, response.data);
        this.$router.push('/accounts');
      })
      .catch((error) => alert('Could not login.'));
  }
}
</script>