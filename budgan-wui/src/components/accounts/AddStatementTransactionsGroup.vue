<template>
  <div class="d-flex flex-row justify-space-between">
    <div>
      <span class="title">Start date : </span>
      <span>{{transactionsGroup.dateStart.toLocaleString()}}</span>
    </div>
    <div>
      <span class="title">End date : </span>
      <span>{{transactionsGroup.dateEnd.toLocaleString()}}</span>
    </div>
  </div>
</template>

<style scoped>
  .title {
    font-weight: bold;  }
</style>

<script setup lang="ts">
  import { type BankAccountTransactionsGroup } from '@models/BankAccountTypes';

  const props = defineProps<{
    transactionsGroup: BankAccountTransactionsGroup
  }>();

</script>
