<template>
  <v-card>
    {{props.title}}
  </v-card>
</template>

<style scoped>
</style>

<script setup lang="ts">
const props = defineProps({
  title: {
    type: String
  }
});
</script>
