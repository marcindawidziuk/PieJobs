<template>
  <div>
    <ul>
      <li class="bg-purple-50 p-2 m-1" v-for="jobDefinition in jobDefinitions">
        <div v-if="editingJobId === jobDefinition.id">
          Name: 
          <input v-model="jobDefinition.name" />
          Command: 
          <input v-model="jobDefinition.command" />
          <button class="bg-blue-500 px-2 rounded text-white" @click="save()">Save</button>
          <button class="bg-red-500 px-2 rounded text-white ml-1" @click="editingJobId = null">Cancel</button>
        </div>
        <div v-else>
          Name: {{jobDefinition.name }}
          Command: {{jobDefinition.command }}
          <button class="bg-blue-500 px-2 rounded text-white" @click="editingJobId = jobDefinition.id">Edit</button>
          <button class="bg-blue-500 px-2 rounded text-white ml-1" @click="schedule(jobDefinition.id)">Schedule</button>
        </div>
      </li>
    </ul>
    <div class="bg-blue-100 p-2">
    Name:<input v-model="newJobName"/>
    Command:<input v-model="newJobCommand"/>
    <button class="bg-blue-500 px-2 rounded text-white ml-1" @click="add">Add</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  AddJobDefinitionDto,
  JobDefinitionDto,
  JobDefinitionsClient,
  JobsClient
} from "../services/api.generated.clients";
import {onMounted, ref} from "vue";

const jobDefinitions = ref<JobDefinitionDto[]>([])
const newJobName = ref("")
const newJobCommand = ref("")

const editingJobId = ref<number | null>()

const save = async function(){
  try {
    const client = new JobDefinitionsClient();
    const selectedJob = jobDefinitions.value.find(x => x.id == editingJobId.value)
    
    const dto = new AddJobDefinitionDto({
      name: selectedJob.name,
      command: selectedJob.command
    });
    await client.edit(dto, editingJobId.value)
  } catch (e) {
    console.log("Failed saving job", e)
    alert("Failed saving job")
  }

}

const init = async function () {
  try {
    const client = new JobDefinitionsClient();
    jobDefinitions.value = await client.getAll()
  } catch (e) {
    console.log("Failed loading job definitions", e)
    alert("Failed loading job definitions")
  }
}

const add = async function () {
  try {
    const client = new JobDefinitionsClient();
    const dto = new AddJobDefinitionDto({
      name: newJobName.value,
      command: newJobCommand.value
    });
    
    await client.add(dto)
    await init();
  } catch (e) {
    console.log("Failed loading job definitions", e)
    alert("Failed loading job definitions")
  }
}

const schedule = async function (jobDefinitionId: number) {
  try {
    const client = new JobsClient();
    await client.schedule(jobDefinitionId)
    await init();
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