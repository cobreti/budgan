<template>
  <v-container class="d-flex flex-wrap align-content-start flex-row">
    <account-tile class="account-tile" v-for="(account) in accounts" :account="account"></account-tile>
  </v-container>
</template>

<style scoped>
  .account-tile {
    display: block;
    position: relative;
    min-height: 10em;
    max-height: 10em;
    min-width: 20em;
    width: 20em;
    margin: 2em;
  }
</style>

<script setup lang="ts">
import AccountTile from '@/components/AccountTile.vue';
import {useBankAccountsStore} from '@/stores/bankAccounts';
import {computed, type ComputedRef} from 'vue';
import type {BankAccountsDictionary} from '@models/BankAccountTypes';

const store = useBankAccountsStore();

const accounts : ComputedRef<BankAccountsDictionary> = computed(() => store.accounts);

</script>
