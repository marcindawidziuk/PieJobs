<template>
  <div>
    <span>UserName</span>
    <input type="text" v-model="userName">
    <span>Password</span>
    <input type="password" v-model="password">
    <button @click="login">Login</button>
  </div>
</template>

<script setup lang="ts">
import {ref} from "vue";
import {LoginRequestDto, UsersClient} from "../services/api.generated.clients";
import {userStore} from "../stores/UserStore";
const userName = ref("")
const password = ref("")

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
    }
  } catch (e) {
    console.log("Failed logging in", e)
    alert("Failed logging in")
  }

}
</script>

<style scoped>

</style>