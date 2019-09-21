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
import VueRouter from 'vue-router';

@Component
export default class LoginForm extends Vue {

  private email: string = '';

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
        alert(jwtToken);
    })
    .catch((error) => alert('error=' + error));
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
