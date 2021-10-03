<script setup>
import {userStore} from "~/stores/UserStore";

if (userStore.getState().token) {
  userStore.refreshUser()
}
</script>

<template>
  <div>
    <header class="bg-white shadow">
      <span class="text-xl inline-block ml-3">PieJobs</span>
      <div class="px-4 py-6 mx-auto max-w-7xl sm:px-6 lg:px-8 inline-block">
        <router-link class="mx-2" to="/">Dashboard</router-link>
        <router-link v-if="userStore.getUser()" class="mx-2" to="/jobs">Jobs</router-link>
        <a href="#" @click="userStore.logout()" class="mx-2" v-if="userStore.getUser()">Logout</a>
        <router-link to="/login" class="mx-2" v-else>
          Login
        </router-link>
        <router-link to="/password" class="mx-2" v-if="userStore.getUser()">Change password</router-link>
      </div>
    </header>
    <main>
      <router-view />
    </main>
  </div>
</template>
