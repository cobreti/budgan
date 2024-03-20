<template>
  account view : {{accountId}}
</template>


<style scoped>
</style>


<script setup lang="ts">
  import {useRoute} from 'vue-router';
  import {computed} from 'vue';
  import {useBankAccountsStore} from '@/stores/bankAccounts-store';
  import {useAccountViewStore} from '@/stores/accountView-store';

  const bankAccountStore = useBankAccountsStore();
  const accountViewStore = useAccountViewStore();

  const route = useRoute();

  const accountId = computed(() => {
    return route.params.id as string;
  });

  const account = computed(() => {
    return bankAccountStore.getAccountById(accountId.value);
  });

  accountViewStore.addBankAccount(account.value);

</script>
