<template>
  <div id="app" class="is-full-height is-light">
    <AppHeader/>
    <section>
      <div class="container is-fullhd">
        <router-view/>
      </div>
    </section>
    <footer class="footer is-hidden-touch">
      <div class="container has-text-centered">
        <p class="is-size-7">
          Copyright {{ currentYear }} © SandBank Inc. All rights reserved.
        </p>
      </div>
    </footer>
  </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import AppHeader from '@/components/AppHeader.vue';

@Component({
  components: {
    AppHeader,
    },
})
export default class App extends Vue {

  private get currentYear() {
    return new Date().getFullYear();
  }

}
</script>

<style>
.is-full-height {
  height: 100vh;
}
.is-light {
  background-color: #fafafa;
}
</style>
