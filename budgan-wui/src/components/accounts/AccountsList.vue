<template>
  <ul>
    <li v-for="account in bankAccountsStore.accounts" :key="account.id">
      {{account.accountId}} - {{account.transactions.length}}
    </li>
  </ul>
</template>

<style>

</style>

<script setup lang="ts">

import {useBankAccountsStore} from '@/stores/bankAccounts';
import {computed} from 'vue';

const bankAccountsStore = useBankAccountsStore();

const bankAccountsLength = computed(() => {
  return Object.keys(bankAccountsStore.accounts).length;
});


</script>