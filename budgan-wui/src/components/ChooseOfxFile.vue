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


<script lang="ts">

import {ServicesTypes} from '@/services/types';
import type {ILocalFilePicker} from '@/services/localFilePicker';

declare global {
  interface Window {
    showOpenFilePicker(options: any): Promise<FileSystemFileHandle[]>
  }
}

export default {
  props: {
  },
  data() {
    return {
      ofxFileName: [] = []
    }
  },
  inject: [
      'container'
  ],
  methods: {
    onFileNameUpdated: (files: File[]) => {
      console.log(files);
    },
    async onLoad() {
      var filePicker : ILocalFilePicker = this.container.get(ServicesTypes.LocalFilePicker);

      var res = await window.showOpenFilePicker({
        types: [
          {
            description: "qfx",
            accept: {
              "text/ofx": [".qfx"]
            }
          }
        ]
      });

      const file = await res[0].getFile();

      const reader = new FileReader();
      // reader.onload = () => {
      //   var content : string = reader.result as string;
      // }

      reader.readAsText(file);

      console.log('loading file ...', res);
    }
  },
  mounted() {

  },
  computed: {
    canLoad() : Boolean {
      return true;
      //return this.ofxFileName.length > 0
    }
  }
}
</script>
