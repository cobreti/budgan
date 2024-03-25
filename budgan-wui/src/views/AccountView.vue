<template>
  <div class="d-flex flex-column align-content-start h-100">
    <div class="test mb-4">
      account view : {{accountId}}
    </div>

    <div class="transactions-list-container mt-2 mb-2">
      <div class="transactions-list">
        <account-view-transaction-list></account-view-transaction-list>
      </div>
    </div>
  </div>
</template>


<style scoped>
  .test {
    display: block;
    position: relative;
  }

  .transactions-list-container {
    flex: 1 1 0;
    display: block;
    position: relative;
    overflow: hidden;
    height: 100%;
  }

  .transactions-list {
    display: block;
    position: relative;
    overflow: auto;
    height: 100%;
  }
</style>


<script setup lang="ts">
  import {useRoute} from 'vue-router';
  import {computed} from 'vue';
  import {useBankAccountsStore} from '@/stores/bankAccounts-store';
  import {useAccountViewStore} from '@/stores/accountView-store';
  import AccountViewTransactionList from '@/components/accountView/AccountViewTransactionList.vue';

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
