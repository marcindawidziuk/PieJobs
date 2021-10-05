<template>
  <div class="mx-3 text-red-400" v-if="errorMessage">{{ errorMessage }}</div>
  <div class="mx-3" v-else-if="isInitialised">
    <div class="flex justify-between">
        <span class="text-white col-auto m-2 text-lg">
          Last jobs:
        </span>
        
        <div class="col float-right">
          <span class="text-white">Show</span>
          <select v-model="maxJobs" class="m-2 py-1">
            <option value="10">10</option>
            <option value="20">20</option>
            <option value="50">50</option>
            <option value="100">100</option>
          </select>
        </div>
    </div>
    <ul v-if="jobs.length">
      <li class="m-1 bg-gray-800 p-3 text-gray-50 hover:bg-gray-600" @click="openJobLogs(job.id)" v-for="job in jobs">
        <span class="font-bold">
          {{ job.jobDefinitionName }}
          <JobStatusPill class="m-2" :job-status="job.status" />
        </span>
        <span class="block text-sm" v-if="job.finishedDateTimeUtc">Finished: {{ displayUtcDate(job.finishedDateTimeUtc)}} in {{ secondsBetweenDates(job.startedDateTimeUtc, job.finishedDateTimeUtc).toFixed(2) }} seconds</span>
        <span class="block text-sm" v-else-if="job.startedDateTimeUtc">Started: {{ displayUtcDate(job.startedDateTimeUtc)}}</span>
        <span class="block text-sm" v-else>Scheduled: {{ displayUtcDate(job.scheduleDateTimeUtc) }}</span>
      </li>
    </ul>
    <div v-else class="text-white">No jobs to show, add and schedule some first</div>
  </div>
  <div v-else class="text-white p-2">
    Loading...
  </div>
</template>

<script setup lang="ts">
import {JobDto, JobsClient} from "../services/api.generated.clients";
import {onBeforeUnmount, onMounted, ref, watch} from "vue";
import {secondsBetweenDates} from "../utils"
import JobStatusPill from "./JobStatusPill.vue";
import {useRouter} from "vue-router";

const jobs = ref<JobDto[]>([])
const maxJobs = ref(10)
const isInitialised = ref(false)
const errorMessage = ref("")
const router = useRouter()

const openJobLogs = function(jobId: number){
  router.push({name: 'ViewLogs', params: {id: jobId}})
}

const displayUtcDate =  function (date: Date){
  if (!date)
    return ""
  const utcDte = new Date(date.toUTCString())
  return `${utcDte.toLocaleTimeString()} ${utcDte.toLocaleDateString()}`
}

const init = async function () {
  try {
    const client = new JobsClient();
    jobs.value = await client.getAll(maxJobs.value)
    isInitialised.value = true;
    errorMessage.value = ""
  } catch (e) {
    console.log("Failed loading jobs", e)
    errorMessage.value = "Failed loading jobs"
  }
}

watch(maxJobs, () => init())

const timer = ref(0)

onMounted(async () => {
  await init()
  timer.value = setInterval(() => init(), 10000)
})

onBeforeUnmount(() => {
    if (timer.value)
      clearTimeout(timer.value)
})

</script>

<style scoped>

</style>