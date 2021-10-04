<template>
  <div>
    <span class="text-gray-50 block m-2">Password</span>
    <input type="password" v-model="password" class="m-2 text-black">
    <span class="text-gray-50 block m-2">Confirm Password</span>
    <input type="password" v-model="passwordConfirmation" class="m-2 text-black">
    <button @click="login" :disabled="!canChangePassword" class="bg-green-400 p-1 rounded block m-2">Change password</button>
  </div>
</template>

<script setup lang="ts">
import {computed, ref} from "vue";
import {UsersClient} from "../services/api.generated.clients";
import {useRouter} from "vue-router";
const password = ref("")
const passwordConfirmation = ref("")

const canChangePassword = computed(() => password.value && password.value == passwordConfirmation.value)
const router = useRouter()

const login = async function () {
  try {
    const client = new UsersClient();
    await client.setPassword(password.value)
    await router.push({name: 'Dashboard'})
  } catch (e) {
    console.log("Failed changed password", e)
    alert("Failed changing password")
  }
}
</script>

<style scoped>

</style>