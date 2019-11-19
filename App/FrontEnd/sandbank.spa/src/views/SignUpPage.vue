<template>
  <div class="columns is-mobile is-centered is-vcentered is-gapless is-full-height">
    <div class="column is-one-quarter-desktop is-three-quarters-mobile">
      <b-field label="Email">
        <b-input 
          type="email"
          placeholder="you@sandbank.com"
          v-model="email"
          icon-pack="fas"
          icon="envelope"
          @keyup.native.enter="signUp">
        </b-input>
      </b-field>
      <a class="button is-primary" @click="signUp">Sign Up</a>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { AxiosResponse } from 'axios';
import { authStore } from '@/store/store';

@Component
export default class SignUpPage extends Vue {
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