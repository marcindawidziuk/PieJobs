<template>
  <span class="p-0.5 px-1.5 rounded px-1 text-sm font-bold" :class="classForJobStatus(jobStatus)">{{ statusDisplay(jobStatus) }}</span>
</template>

<script setup lang="ts">
import {statusDisplay} from "../utils"
import {JobStatus} from "../services/api.generated.clients";
import {PropType} from "vue";

const classForJobStatus = function (status: JobStatus) {
  if (status === JobStatus.Completed)
    return 'bg-green-300 text-green-900'
  if (status === JobStatus.InProgress)
    return 'bg-blue-300 text-blue-900'
  if (status === JobStatus.Failed)
    return 'bg-red-300 text-red-900'
  if (status === JobStatus.Cancelled)
    return 'bg-indigo-300 text-indigo-800'
  return 'bg-gray-300 text-gray-800'
}


defineProps({
  jobStatus:{
    type: Number as PropType<JobStatus>,
    required: true
  }
})
</script>

<style scoped>

</style>