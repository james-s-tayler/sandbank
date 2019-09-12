import Vue from 'vue';
import Router from 'vue-router';
import SignUpPage from './views/SignUpPage.vue';

Vue.use(Router);

export default new Router({
  routes: [
    {
      path: '/',
      name: 'register',
      component: SignUpPage,
    },
    {
      path: '/login',
      name: 'login',
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "about" */ './views/SignInForm.vue'),
    },
  ],
});
