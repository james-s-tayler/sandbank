<template>
  <el-form status-icon label-width="120px" class="demo-ruleForm">
    <el-form-item label="Email" prop="email">
      <el-input type="email" v-model="email"></el-input>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="signUp()">Register</el-button>
    </el-form-item>
  </el-form>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import Axios, { AxiosResponse } from 'axios';
import VueRouter from 'vue-router';

@Component
export default class HelloWorld extends Vue {

  private email: string = '';

  public signUp() {
    const registerUserRequest = {
      fullName: 'John Smith',
      email: this.email,
      phone: '0211234567',
      dateOfBirth: '1999-09-11T10:27:25.362Z',
      address: '123 Fake St',
      country: 'NZ',
      postCode: '1024',
      city: 'Auckland',
    };

    const headers = {
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    };

    this.$http.post('/user', registerUserRequest, headers)
      .then((response: AxiosResponse) => {

        const user = response.data;

        const openAccountRequest = {
          accountType: 'TRANSACTION',
          displayName: 'My Account',
        };

        this.$http.post('/user/account', openAccountRequest, headers)
        .then((openAccountReponse: AxiosResponse) => {
          const accountId = openAccountReponse.data.id;

          this.$http.post('/user/account/' + accountId + '/seed', {}, headers)
          .then((seedTransactionsResponse: AxiosResponse) => {
            this.$router.push('/user/account');
          });
        });
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
