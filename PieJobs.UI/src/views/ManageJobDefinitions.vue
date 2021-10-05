<template>
  <div class="m-2">
    <ul>
      <li class="my-3 bg-gray-800 p-3 text-gray-50 border-b-2 border-gray-600 border-r-2" v-for="jobDefinition in jobDefinitions">
        <div v-if="editingJobId === jobDefinition.id">
          <div class="block mt-1">
          Name: 
          <input v-model="jobDefinition.name" class="text-white px-1 py-0.5 bg-gray-600" />
          </div>
          <div class="block my-2">
            Command: 
            <input v-model="jobDefinition.command" class="text-white px-1 py-0.5 bg-gray-600" />
          </div>
          <button class="bg-blue-500 px-3 py-1 rounded text-white" @click="save()">Save</button>
          <button class="bg-red-500 px-3 py-1 ml-2 rounded text-white" @click="editingJobId = null; init()">Cancel</button>
        </div>
        <div v-else>
          <span class="block">Name: {{jobDefinition.name }}</span>
          <span class="block mb-1">Command: {{jobDefinition.command }}</span>
          <button class="bg-blue-500 px-3 py-1 rounded text-white" @click="editingJobId = jobDefinition.id">Edit</button>
          <button class="bg-yellow-600 text-indigo-50 px-3 py-1 rounded text-white ml-5 float-right" @click="schedule(jobDefinition.id)">▶ Run </button>
        </div>
      </li>
    </ul>
    <div class="bg-gray-800 p-3 text-gray-50 border-b-2 border-gray-600 border-r-2">
      <span class="block mb-2 text-lg">Adding new command:</span>
      <span>Name: </span>
      <input v-model="newJobName" class="text-white px-1 py-0.5 bg-gray-600" />
      <span class="ml-2">Command: </span>
      <input v-model="newJobCommand" class="text-white px-1 py-0.5 bg-gray-600" />
      <button class="bg-blue-500 px-3 py-1 rounded text-white ml-2" @click="add()">Add job</button>
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
import {useRouter} from "vue-router";

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
    editingJobId.value = null;
    await init()
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
    if (!newJobName.value || !newJobCommand.value){
      alert("Enter name and command")
      return
    }
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

const router = useRouter()

const schedule = async function (jobDefinitionId: number) {
  try {
    const client = new JobsClient();
    await client.schedule(jobDefinitionId)
    await router.push({name: 'Dashboard'})
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