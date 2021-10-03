<template>
  <div>
    Jobs:
    <ul>
      <li class="m-1 bg-blue-100 p-2" v-for="job in jobs">
        <span class="font-bold">
          {{ job.jobDefinitionName }}
        </span>
        <span class="font-light pl-2 text-sm">{{ job.command }}</span>
        <span class="block">Status: {{ statusDisplay(job.status) }}</span>
        <span class="block">Scheduled: {{ displayUtcDate(job.scheduleDateTimeUtc) }}</span>
        <span class="block" v-if="job.startedDateTimeUtc">Started: {{ displayUtcDate(job.startedDateTimeUtc)}}</span>
        <span class="block" v-if="job.finishedDateTimeUtc">Finished: {{ displayUtcDate(job.finishedDateTimeUtc)}}</span>
      </li>
    </ul>

    Definitions
    <ul>
      <li class="bg-blue-50 p-2 m-1" v-for="jobDefinition in jobDefinitions">
        {{jobDefinition.name }}
        <button class="bg-blue-500 px-2 rounded text-white" @click="schedule(jobDefinition.id)">Schedule</button>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import {JobDefinitionDto, JobDefinitionsClient, JobDto, JobsClient, JobStatus} from "../services/api.generated.clients";
import {onMounted, ref} from "vue";

const jobDefinitions = ref<JobDefinitionDto[]>([])
const jobs = ref<JobDto[]>([])

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
    const jobDefinitionsClient = new JobDefinitionsClient();
    jobDefinitions.value = await jobDefinitionsClient.getAll()
    const client = new JobsClient();
    jobs.value = await client.get()
  } catch (e) {
    console.log("Failed loading job definitions", e)
    alert("Failed loading job definitions")
  }
}

onMounted(async () => {
  await init()
})

</script>

<style scoped>

</style>