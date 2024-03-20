<template>
  <v-card>
    <div class="d-block, text-center font-weight-bold mt-2 mb-4">
      {{props.account?.accountId}}
    </div>
    <div class="d-flex justify-space-between ml-2 mr-2">
      <span class="">
        {{minDate?.toLocaleDateString()}}
      </span>
      <span class="">
        {{maxDate?.toLocaleDateString()}}
      </span>
    </div>
    <div class="d-block ma-2">
      Imported ofx count : {{props.account?.transactions.length}}
    </div>
    <div class="text-center">
      <RouterLink :to="accountViewLink">View</RouterLink>
    </div>
  </v-card>
</template>

<style scoped>
</style>

<script setup lang="ts">

import type {BankAccount} from '@models/BankAccountTypes';
import {computed} from 'vue';

type AccountTileProps = {
  title?: string;
  account?: BankAccount;
}

const props = defineProps<AccountTileProps>();

const accountViewLink = computed(() => {
  return `/account/${props.account?.accountId}`;
});

const minDate = computed(() => {
  if (props.account) {
    return props.account.transactions.reduce((min, p) => p.dateStart < min ? p.dateStart : min, props.account.transactions[0].dateStart);
  }
  return undefined;
});

const maxDate = computed(() => {
  if (props.account) {
    return props.account.transactions.reduce((max, p) => p.dateEnd > max ? p.dateEnd : max, props.account.transactions[0].dateEnd);
  }
  return undefined;
});

</script>
