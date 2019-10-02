import Vue from 'vue';
import Router, { RouteRecord } from 'vue-router';
import HomePage from './views/HomePage.vue';
import SignUpPage from './views/SignUpPage.vue';
import LoginPage from './views/LoginPage.vue';
import Transactions from './views/Transactions.vue';
import Accounts from './views/Accounts.vue';
import NotFound from './views/NotFound.vue';
import store, { authStore } from '@/store/store';

Vue.use(Router);

export const router = new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomePage,
    },
    {
      path: '/register',
      name: 'register',
      component: SignUpPage,
      meta: { redirectIfAuthenticated: true },
    },
    {
      path: '/login',
      name: 'login',
      component: LoginPage,
      meta: { redirectIfAuthenticated: true },
    },
    {
      path: '/account/:accountId',
      name: 'transactions',
      component: Transactions,
      meta: { requiresAuth: true },
    },
    {
      path: '/accounts',
      name: 'accounts',
      component: Accounts,
      meta: { requiresAuth: true },
    },
    {
      path: '*',
      component: NotFound,
    },
  ],
});

router.beforeEach((to, from, next) => {

  const expiration = window.sessionStorage.getItem('authTokenExpiration');
  const unixTimestamp = new Date().getUTCMilliseconds() / 1000;
  store.commit(`${authStore}/updateAuthStatus`, expiration !== null && parseInt(expiration, 10) - unixTimestamp > 0);

  if (to.matched.some((record: RouteRecord) => record.meta.requiresAuth)) {
    // this route requires auth, check if logged in
    // if not, redirect to login page.
    if (!store.getters[`${authStore}/isAuthenticated`]) {
      next({
        name: 'login',
      });
    } else {
      next();
    }
  } else if (to.matched.some((record: RouteRecord) => record.meta.redirectIfAuthenticated)) {
    if (store.getters[`${authStore}/isAuthenticated`]) {
      next({
        name: 'accounts',
      });
    } else {
      next();
    }
  } else {
    next();
  }
});
