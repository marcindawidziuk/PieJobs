<template>
  <div class="bg-gray-800 m-2 p-2 border-b-2 border-r-2 border-gray-600">
    <div class="bg-gray-800 border-gray-400 m-1 p-0 text-gray-100 border-b-2 pb-2 border-gray-700">
      <span class="my-2 text-white text-lg">{{jobDetails.jobDefinitionName}}</span>
      <JobStatusPill class="mx-2" :job-status="jobDetails.status" />
      <span class="block text-md">Scheduled: {{ displayUtcTime(jobDetails.scheduleDateTimeUtc) }} {{ displayUtcDate(jobDetails.scheduleDateTimeUtc) }}</span>
      <span class="block text-md" v-if="jobDetails.startedDateTimeUtc">Started: {{ displayUtcTime(jobDetails.startedDateTimeUtc)}}</span>
      <span class="block text-md" v-if="jobDetails.finishedDateTimeUtc">Finished: {{ displayUtcTime(jobDetails.finishedDateTimeUtc)}} in {{ secondsBetweenDates(jobDetails.startedDateTimeUtc, jobDetails.finishedDateTimeUtc).toFixed(2) }} seconds</span>
    </div>
    <span v-if="logLines.length === 0" class="text-red-100">No logs to display</span>
    <div v-for="log in logLines" class="block text-sm">
      <span :class="log.isError ? 'text-red-300' : 'text-gray-50'">
        {{ displayUtcTime(log.dateTimeUtc) }} {{ log.text }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import {computed, onBeforeUnmount, onMounted, ref} from "vue";
import {JobDto, JobsClient, JobStatus, LogLineDto, LogsClient} from "../services/api.generated.clients";
import {useRoute} from "vue-router";
import JobStatusPill from "./JobStatusPill.vue";
import {secondsBetweenDates} from "../utils"

const displayUtcTime =  function (date: Date){
  if (!date)
    return ""
  const utcDte = new Date(date.toUTCString())
  return `${utcDte.toLocaleTimeString()}` 
}

const displayUtcDate =  function (date: Date){
  if (!date)
    return ""
  const utcDte = new Date(date.toUTCString())
  return `${utcDte.toLocaleDateString()}`
}

const logLines = ref<LogLineDto[]>([])
const jobDetails = ref<JobDto>(new JobDto())

const route = useRoute()

const jobId = computed(() => parseInt(route.params.id.toString()))

const init = async function(){
  try {
    const jobsClient = new JobsClient();
    jobDetails.value = await jobsClient.get(jobId.value)
    
    const logsClient = new LogsClient();
    logLines.value = await logsClient.getForJob(jobId.value)
  } catch (e) {
    console.log("Failed loading logs", e)
    alert("Failed loading logs")
  }
}

onMounted(() => {
  init()
  timer.value = setInterval(() =>{ 
    if (jobDetails.value.status == JobStatus.InProgress 
        || jobDetails.value.status == JobStatus.Pending){
      init()
    }
  }, 1000)
})

const timer = ref(0)

onBeforeUnmount(() => {
  if (timer.value)
    clearTimeout(timer.value)
})
</script>