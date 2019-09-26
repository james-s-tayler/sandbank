<template>
  <div style="width: 40%; margin: auto; margin-top: 40px;">
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
