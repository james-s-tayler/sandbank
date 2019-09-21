<template>
  <div class="home">
    <div v-if="isAuthenticated">
      <p>You are logged in.</p>
    </div>
    <div v-else>
      <form>
        <label for="email">Email</label>
        <input v-model="email" name="email" type="email">
        <input @click="login()" value="Login">
      </form>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import Axios, { AxiosResponse } from 'axios';
import VueRouter from 'vue-router';
import { JwtHelper } from '@/jwt-helper';

@Component
export default class LoginForm extends Vue {

  private email: string = '';
  private isAuthenticated: boolean = false;

  public login(): void {

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

    this.$http.post('/user/login', loginUserRequest, headers)
      .then((response: AxiosResponse) => {
        const jwtToken = response.data;
        const parsedToken = new JwtHelper().decodeToken(jwtToken);

        window.localStorage.setItem('authToken', jwtToken);
        window.localStorage.setItem('authTokenExpiration', parsedToken.exp);
    })
    .catch((error) => alert('Could not login.'));
  }

  private created() {
    const expiration = window.localStorage.getItem('authTokenExpiration');
    const unixTimestamp = new Date().getUTCMilliseconds() / 1000;

    if (expiration !== null && parseInt(expiration, 10) - unixTimestamp > 0) {
      this.isAuthenticated = true;
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
