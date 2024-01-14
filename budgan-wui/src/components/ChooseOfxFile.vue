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
// const ofxFileModel = defineModel();
export default {
  props: {
  },
  data() {
    return {
      ofxFileName: [] = []
    }
  },
  methods: {
    onFileNameUpdated: (files: []) => {
      console.log(files);
    },
    onLoad: () => {
      console.log('loading file ...');
    }
  },
  mounted() {

  },
  computed: {
    canLoad() : Boolean {
      return this.ofxFileName.length > 0
    }
  }
}
</script>
