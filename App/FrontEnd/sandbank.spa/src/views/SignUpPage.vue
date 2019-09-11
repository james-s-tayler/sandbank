<template>
  <div>
    <h1>Register</h1>
    <SignUpForm />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import SignUpForm from '@/components/SignUpForm.vue'; // @ is an alias to /src

@Component({
  components: {
    SignUpForm,
  },
})
export default class SignUpPage extends Vue {}
</script>
