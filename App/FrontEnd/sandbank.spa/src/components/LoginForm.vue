<template>
  <el-form status-icon label-width="120px" class="demo-ruleForm">
    <el-form-item label="Email" prop="email">
      <el-input type="email" v-model="email"></el-input>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="login()">Login</el-button>
    </el-form-item>
  </el-form>
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

        this.$store.commit('updateAuthStatus', true);
        this.$router.push('/accounts');
      })
      .catch((error) => alert('Could not login.'));
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
