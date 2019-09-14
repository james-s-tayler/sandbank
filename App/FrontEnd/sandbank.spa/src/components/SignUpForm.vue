<template>
  <div class="home">
    <form>
      <label for="email">Email</label>
      <input v-model="email" name="email" type="email">
      <input @click="signUp()" value="Sign Up">
    </form>
  </div>
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

    // this needs a clean up - should set axios baseURL and externalize configuration
    // probably need to tidy up promises too

    this.$http.post('/user', registerUserRequest, headers)
      .then((response: AxiosResponse) => {

        const user = response.data;
        const userId = user.id;

        const openAccountRequest = {
          accountType: 'TRANSACTION',
          displayName: 'My Account',
        };

        this.$http.post('/user/' + userId + '/account', openAccountRequest, headers)
        .then((openAccountReponse: AxiosResponse) => {
          const accountId = openAccountReponse.data.id;

          this.$http.post('/user/' + userId + '/account/' + accountId + '/seed', {}, headers)
          .then((seedTransactionsResponse: AxiosResponse) => {
            this.$router.push('/user/' + userId + '/account/' + accountId);
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
