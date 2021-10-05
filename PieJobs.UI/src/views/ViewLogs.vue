<template>
  <div class="bg-gray-800 m-2 p-2">
    <span class="my-2 text-white text-lg">{{jobDetails.jobDefinitionName}}</span>
    <JobStatusPill class="mx-2" :job-status="jobDetails.status" />
    <span v-if="logLines.length === 0" class="text-red-100">No logs to display</span>
    <div v-for="log in logLines" class="block">
      <span :class="log.isError ? 'text-red-300' : 'text-gray-50'">
        {{ displayUtcDate(log.dateTimeUtc) }} {{ log.text }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import {computed, onMounted, ref} from "vue";
import {JobDto, JobsClient, LogLineDto, LogsClient} from "../services/api.generated.clients";
import {useRoute} from "vue-router";
import JobStatusPill from "./JobStatusPill.vue";

const displayUtcDate =  function (date: Date){
  if (!date)
    return ""
  const utcDte = new Date(date.toUTCString())
  return `${utcDte.toLocaleTimeString()}` 
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
})
</script>