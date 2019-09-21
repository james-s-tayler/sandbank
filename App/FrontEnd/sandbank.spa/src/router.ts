import Vue from 'vue';
import Router from 'vue-router';
import SignUpPage from './views/SignUpPage.vue';
import LoginPage from './views/LoginPage.vue';
import Transactions from './views/Transactions.vue';
import Accounts from './views/Accounts.vue';
import NotFound from './views/NotFound.vue';

Vue.use(Router);

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      name: 'register',
      component: SignUpPage,
    },
    {
      path: '/login',
      name: 'login',
      component: LoginPage,
    },
    {
      path: '/user/account/:accountId',
      name: 'transactions',
      component: Transactions,
    },
    {
      path: '/user/account',
      name: 'accounts',
      component: Accounts,
    },
    {
      path: '*',
      component: NotFound,
    },
  ],
});
