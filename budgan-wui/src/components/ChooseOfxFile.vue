<template>
  <div>
    <v-card class="pt-4 pr-4 pl-4 pb-4 mb-8">
      <v-file-input
          v-model="ofxFileName"
          @update:modelValue="onFileNameUpdated"
          accept=".ofx"
        ></v-file-input>
    </v-card>
    <v-col class="text-center">
      <v-btn
          :disabled="!canLoad"
          @click="onLoad"
        >
        Load
      </v-btn>
    </v-col>
  </div>
</template>


<style scoped>

</style>


<script setup lang="ts">

import {computed, inject} from 'vue';
import type {Container} from 'inversify';
import {ServicesTypes} from '@/services/types';
import type {IOfxParser} from '@/services/ofxParser';

const container = inject('container') as Container;
const ofxFileName = defineModel<File[]>();
const canLoad = computed(() => {
  return ofxFileName.value && ofxFileName.value.length > 0
});

function onLoad(event: Event) {

  if (!ofxFileName.value)
    return;

  const reader = new FileReader();

  reader.onload = () => {
    const content : string = reader.result as string;
    const ofxParser : IOfxParser = container.get(ServicesTypes.OfxParser);

    if (ofxParser) {
      ofxParser.parse(content);
    }

    // console.log(content);
  };

  reader.readAsText(ofxFileName.value[0]);
}

function onFileNameUpdated(files: File[]) {

}

</script>

