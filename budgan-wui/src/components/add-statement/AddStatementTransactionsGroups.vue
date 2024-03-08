<template>
  <div class="container ma-4 mt-4 h-100">
    <add-statement-transactions-group v-for="transactionsGroup in transactionsGroups" :transactions-group="transactionsGroup"></add-statement-transactions-group>
  </div>
</template>

<style scoped>
  .container {
    min-height: 20em;
    height: 100%;
  }
</style>


<script setup lang="ts">

  import {useAddStatementStore} from '@/stores/add-statement-store';
  import {computed} from 'vue';
  import AddStatementTransactionsGroup from '@/components/add-statement/AddStatementTransactionsGroup.vue';

  const addStatementStore = useAddStatementStore();

  const transactionsGroups = computed(() => {
    return addStatementStore.loadedAccount.account?.transactions;
  });


</script>
