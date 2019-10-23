import Vue from 'vue';
import App from './App.vue';
import { router } from './router';
import axios, { AxiosRequestConfig } from 'axios';
import VueAxios from 'vue-axios';
import ElementUI from 'element-ui';
import Buefy from 'buefy';
import 'buefy/dist/buefy.css';
import store, { authStore } from '@/store/store';

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
Vue.use(Buefy);

Vue.filter('asCurrency', (value: string, isoCurrency: string) => {
  const moneyFormatter = new Intl.NumberFormat(store.getters[`${authStore}/locale`], {
    style: 'currency',
    currency: isoCurrency,
   });

  let num = Number(value);

  if (Number.isNaN(num)) {
    num = 0;
  }

  return moneyFormatter.format(num);
});

Vue.filter('asDate', (value: string) => {
  const dateFormatter = new Intl.DateTimeFormat(store.getters[`${authStore}/locale`], {
    // options here?
  });
  return dateFormatter.format(new Date(value));
});

Vue.filter('prepend', (value: string, prependThis: string) => {
  return prependThis + value;
});

Vue.filter('splice', (value: string, prependThis: string, appendThis: string) => {
  return prependThis + value + appendThis;
});

const app = new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount('#app');

export { app, router };
