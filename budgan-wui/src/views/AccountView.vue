<template>
  account view : {{accountId}}
</template>


<style scoped>
</style>


<script setup lang="ts">
  import {useRoute} from 'vue-router';
  import {computed} from 'vue';

  const route = useRoute();

  const accountId = computed(() => {
    return route.params.id;
  });
</script>
