<template>
  <div class="text-gray-50 px-2">
    <span class="block m-1">Name</span>
    <input type="text" v-model="userName" class="m-1 text-black">
    <span class="block m-1">Password</span>
    <input type="password" v-model="password" class="m-1 text-black" @keyup.enter="login()">
    <button @click="login" class="bg-green-500 p-1 px-4 rounded block m-2">Login</button>
    <span class="text-red-400 mx-2">{{ errorMessage }}</span>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";
import {LoginRequestDto, UsersClient} from "../services/api.generated.clients";
import {userStore} from "../stores/UserStore";
import {useRouter} from "vue-router";
const userName = ref("")
const password = ref("")
const errorMessage = ref("")
const router = useRouter()

const login = async function () {
  try {
    const client = new UsersClient();
    const request = new LoginRequestDto({
      userName: userName.value,
      password: password.value
    })
    const result = await client.login(request)
    if (result.isSuccessful) {
      userStore.setToken(result.apiToken)
      await userStore.refreshUser()
      await router.push({name: 'Dashboard'})
    }else{
      errorMessage.value = "Invalid credentials"
    }
  } catch (e) {
    console.log("Failed logging in", e)
    errorMessage.value = "Failed logging in"
  }

}
</script>

<style scoped>

</style>