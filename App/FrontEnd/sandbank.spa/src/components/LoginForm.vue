<template>
  <div class="home">
    <form>
      <label for="email">Email</label>
      <input v-model="email" name="email" type="email">
      <input @click="login()" value="Login">
    </form>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import Axios, { AxiosResponse } from 'axios';
import { eventBus } from '@/event-bus';
import VueRouter from 'vue-router';
import { JwtHelper } from '@/jwt-helper';

@Component
export default class LoginForm extends Vue {
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
        const jwtToken = response.data;
        const parsedToken = new JwtHelper().decodeToken(jwtToken);

        window.sessionStorage.setItem('authToken', jwtToken);
        window.sessionStorage.setItem('authTokenExpiration', parsedToken.exp);
        this.email = '';

        eventBus.$emit('authStatusUpdated', true);
      })
      .catch((error) => alert('Could not login.'));
  }

  private created() {
    const expiration = window.sessionStorage.getItem('authTokenExpiration');
    const unixTimestamp = new Date().getUTCMilliseconds() / 1000;

    if (expiration !== null && parseInt(expiration, 10) - unixTimestamp > 0) {
      eventBus.$emit('authStatusUpdated', true);
    }
  }
}
</script>

<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
