<template>
  <div>
    <div class="d-flex flex-column align-content-start ma-1">
      <div class="d-flex flex-row justify-space-between">
        <div>
          <span class="title">Account # : </span>
          <span>{{account.accountId}}</span>
        </div>
        <div>
          <span class="title">Account type : </span>
          <span>{{account.accountType}}</span>
        </div>
      </div>
      <add-statement-transactions-groups class="mt-2"></add-statement-transactions-groups>
    </div>
  </div>
</template>

<style scoped>
  .title {
    font-weight: bold;  }
</style>

<script setup lang="ts">

  import {useAddStatementStore} from '@/stores/add-statement-store';
  import {computed} from 'vue';
  import AddStatementTransactionsGroups from '@/components/accounts/AddStatementTransactionsGroups.vue';

  const addStatementStore = useAddStatementStore();

  const account = computed(() => {
    if (!addStatementStore.loadedAccount.account) {
      throw new Error("Account not loaded")
    }

    return addStatementStore.loadedAccount.account;
  });

</script>