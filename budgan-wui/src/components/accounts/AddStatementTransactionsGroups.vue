<template>
  <div class="container ma-4 mt-4">
    <add-statement-transactions-group v-for="transactionsGroup in transactionsGroups" :transactions-group="transactionsGroup"></add-statement-transactions-group>
  </div>
</template>

<style scoped>
  .container {
    height: 20em;
  }
</style>


<script setup lang="ts">

  import {useAddStatementStore} from '@/stores/add-statement-store';
  import {computed} from 'vue';
  import AddStatementTransactionsGroup from '@/components/accounts/AddStatementTransactionsGroup.vue';

  const addStatementStore = useAddStatementStore();

  const transactionsGroups = computed(() => {
    if (!addStatementStore.loadedAccount.account) {
      throw new Error("Transactions group not loaded")
    }

    return addStatementStore.loadedAccount.account.transactions;
  });


</script>
