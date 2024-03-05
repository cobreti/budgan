<template>
  <div class="h-100 pb-2">
    <div class="d-flex flex-row justify-space-between pb-2">
      <div>
        <span class="title">Start date : </span>
        <span>{{transactionsGroup.dateStart.toLocaleString()}}</span>
      </div>
      <div>
        <span class="title">End date : </span>
        <span>{{transactionsGroup.dateEnd.toLocaleString()}}</span>
      </div>
    </div>
    <div class="list h-100 pa-2">
      <div class="transactions-list-container h-100">
        <div class="d-flex flex-column justify-start h-100">
          <add-statement-transaction class="pt-1 pb-1 transaction" v-for="transaction in transactionsGroup.transactions" :key="transaction.transactionId" :transaction="transaction"></add-statement-transaction>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
  .title {
    font-weight: bold;  }

  .transactions-list-container {
    overflow: auto;
  }

  .list {
    flex-grow: 1;
  }

  .transaction {
  }

  .transaction:nth-child(odd) {
    background-color: rgba(var(--v-theme-on-surface-variant));
  }
</style>

<script setup lang="ts">
  import { type BankAccountTransactionsGroup } from '@models/BankAccountTypes';
  import AddStatementTransaction from '@/components/accounts/AddStatementTransaction.vue';

  const props = defineProps<{
    transactionsGroup: BankAccountTransactionsGroup
  }>();

</script>
