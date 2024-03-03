<template>
  <div class="page-container">
    <div class="d-flex flex-column align-content-start ma-2">
      <v-card class="file-input-card pt-4 pr-4 pl-4 pb-4 mr-2 mb-2">
        <label for="ofx-file-input">
          Choose an OFX file
        </label>
        <v-file-input
            id="ofx-file-input"
            class=""
            v-model="ofxFileName"
            :disabled = "addStatementStore.loadedAccount.loading"
            @update:modelValue="onFileNameUpdated"
            accept=".ofx"
        ></v-file-input>
      </v-card>
      <v-card class="pt-4 pr-4 pl-4 pb-4 mr-2 mt-2" v-if="addStatementStore.loadedAccount.account != undefined">
        <add-statement-account></add-statement-account>
      </v-card>
    </div>
  </div>

</template>

<style>
  .page-container {
    height: 100vh;
  }

  .file-input-card {
    flex: 1 1 0;
    display: block;
    position: relative;
    min-height: 10em;
  }

</style>

<script setup lang="ts">
  import {useAddStatementStore} from '@/stores/add-statement-store';

  const ofxFileName = defineModel<File[]>();
  import AddStatementAccount from '@/components/accounts/AddStatementAccount.vue';
  const addStatementStore = useAddStatementStore();

  function onFileNameUpdated(files: File[]) {

    if (files.length > 0) {
      addStatementStore.loadOfxFile(files[0]);
    }
  }
</script>

