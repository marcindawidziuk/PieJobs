import Home from './views/Home.vue'
import About from './views/About.vue'
import NotFound from './views/NotFound.vue'

/** @type {import('vue-router').RouterOptions['routes']} */
export const routes = [
  // { path: '/', component: Home, meta: { title: 'Home' } },
  { path: '/', name: 'Dashboard', component: () => import("./views/Dashboard.vue"), meta: { title: 'Home' } },
  { path: '/jobs', component: () => import("./views/ManageJobDefinitions.vue"), meta: { title: 'Job Definitions' } },
  { path: '/login', component: () => import("./views/Login.vue"), meta: { title: 'Login' } },
  { path: '/logs/:id', name: 'ViewLogs', component: () => import("./views/ViewLogs.vue"), meta: { title: 'Logs' } },
  { path: '/password', component: () => import("./views/ChangePassword.vue"), meta: { title: 'Change Password' } },
  {
    path: '/about',
    meta: { title: 'About' },
    component: About,
  },
  { path: '/:path(.*)', component: NotFound },
]
