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
      <li class="m-1 bg-gray-800 p-3 text-white" v-for="job in jobs">
        <span class="font-bold">
          {{ job.jobDefinitionName }}
          <span class="mx-1 p-0.5 rounded" :class="classForJobStatus(job.status)">{{ statusDisplay(job.status) }}</span>
        </span>
        <span class="block text-sm" v-if="job.finishedDateTimeUtc">Finished: {{ displayUtcDate(job.finishedDateTimeUtc)}}</span>
        <span class="text-sm" v-if="job.finishedDateTimeUtc">Took: {{ secondsBetweenDates(job.startedDateTimeUtc, job.finishedDateTimeUtc)}} seconds</span>
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
import {JobDto, JobsClient, JobStatus} from "../services/api.generated.clients";
import {onBeforeUnmount, onMounted, ref} from "vue";

const jobs = ref<JobDto[]>([])
const maxJobs = ref(10)
const isInitialised = ref(false)
const errorMessage = ref("")

const classForJobStatus = function (status: JobStatus) {
  if (status === JobStatus.Completed)
    return 'bg-green-700'
  if (status === JobStatus.InProgress)
    return 'bg-blue-700'
  if (status === JobStatus.Failed)
    return 'bg-red-700'
  if (status === JobStatus.Cancelled)
    return 'bg-indigo-700'
  return 'bg-gray-700'
}

const secondsBetweenDates = function (date1: Date, date2: Date){
  return (date2 - date1) / 1000;
}

const statusDisplay = function (status: JobStatus){
  if (status == JobStatus.Pending)
    return "Pending"
  if (status == JobStatus.InProgress)
    return "In Progress"
  if (status == JobStatus.Completed)
    return "Completed"
  return "Failed"
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
    jobs.value = await client.get()
    isInitialised.value = true;
  } catch (e) {
    console.log("Failed loading jobs", e)
    errorMessage.value = "Failed loading jobs"
  }
}

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