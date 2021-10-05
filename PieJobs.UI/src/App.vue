<script setup>
import {userStore} from "~/stores/UserStore";

if (userStore.getState().token) {
  userStore.refreshUser()
}
</script>

<template>
  <div class="flex flex-col h-screen justify-between">
    <header class="bg-gray-800 shadow text-white">
      <router-link class="text-xl inline-block ml-4 text-white hover:opacity-75" to="/">
        <img src="/pie.png" class="inline h-7" alt="pie"/> Jobs
      </router-link>
      <div class="px-2 py-6 mx-auto max-w-7xl sm:px-6 lg:px-8 inline-block">
        <div class="flex justify-between">
          <router-link v-if="userStore.getUser()" class="mr-2 hover:text-gray-300" to="/jobs">Configuration</router-link>
          <a href="#" @click="userStore.logout()" class="mx-2 hover:text-gray-300" v-if="userStore.getUser()">Logout</a>
          <router-link to="/login" class="mx-2 hover:text-gray-300" v-else>
            Login
          </router-link>
          <router-link to="/password" class="hidden mx-2" v-if="userStore.getUser()">Change password</router-link>
        </div>
      </div>
    </header>
    <main class="mb-auto">
      <router-view />
    </main>
    <footer class="text-gray-400 m-1">
      <router-link v-if="userStore.getUser()" to="/password" class="mx-2">Change password</router-link>
      <a href="https://github.com/marcindawidziuk/PieJobs" class="mx-2">Github</a>
  </footer>
  </div>
</template>
