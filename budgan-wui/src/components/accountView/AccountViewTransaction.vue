<template>
  <div class="d-flex flex-row justify-start">
    <div class="date mr-4">
      <span>{{transaction.date.toLocaleString()}}</span>
    </div>
    <div class="type mr-4">
      <span>{{transaction.type}}</span>
    </div>
    <div class="description mr-4">
      <span>{{transaction.description}}</span>
    </div>
    <div class="amount mr-4">
      <span>{{transaction.amount}}</span>
    </div>
  </div>
</template>

<style scoped>
  .date {
    min-width: 15em;
  }

  .type {
    min-width: 10em;
  }

  .description {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    flex-grow: 1;
    min-width: 10em;
  }

  .amount {
    min-width: 10em;
    text-align: right;
  }
</style>

<script setup lang="ts">

  import type {AccountViewTransaction} from '@/stores/accountView-store';

  const props = defineProps<{
    transaction: AccountViewTransaction
  }>();

</script>
