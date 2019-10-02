import Vue from 'vue';
import App from './App.vue';
import { router } from './router';
import axios, { AxiosRequestConfig } from 'axios';
import VueAxios from 'vue-axios';
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import store from '@/store/store';

Vue.config.productionTip = false ;

// need to exernalize configuration for this
axios.defaults.baseURL = 'http://localhost:5100/api';
axios.interceptors.request.use((config: AxiosRequestConfig) => {

  // detect sever side rendering
  if (typeof window === undefined) {
    return config;
  }

  const authToken = window.sessionStorage.getItem('authToken');
  if (authToken) {
    config.headers.Authorization = `Bearer ${authToken}`;
  }
  return config;
});

Vue.use(VueAxios, axios);
Vue.use(ElementUI, {});

const app = new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount('#app');

export { app, router };
