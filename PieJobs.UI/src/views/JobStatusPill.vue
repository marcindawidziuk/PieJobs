<template>
  <span class="p-0.5 rounded px-1 text-white" :class="classForJobStatus(jobStatus)">{{ statusDisplay(jobStatus) }}</span>
</template>

<script setup lang="ts">
import {statusDisplay} from "../utils"
import {JobStatus} from "../services/api.generated.clients";
import {PropType} from "vue";

const classForJobStatus = function (status: JobStatus) {
  if (status === JobStatus.Completed)
    return 'bg-green-600'
  if (status === JobStatus.InProgress)
    return 'bg-blue-700'
  if (status === JobStatus.Failed)
    return 'bg-red-700'
  if (status === JobStatus.Cancelled)
    return 'bg-indigo-700'
  return 'bg-gray-700'
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