<template>
  <div class="transactions-block-panel">
    <div v-for="transactionsblock in bankAccountTransactions">
      <transactions-list :transactions="transactionsblock.transactions"></transactions-list>
    </div>
  </div>
</template>

<style scoped>
  .transactions-block-panel {
    position: relative;
    display: block;
    padding-left: 1em;
    //max-height: 8em;
    overflow: auto;

    ul {
      list-style: none;
      padding: 0;
      margin: 0;

      li {
        font-size: smaller;
        padding-top: 0.10em;
        padding-bottom: 0.10em;
      }

      li:nth-child(odd) {
        background-color: lightblue;
      }
    }
  }
</style>


<script setup lang="ts">
  import type {BankAccountTransactions} from '@/stores/bankAccounts';
  import TransactionsList from '@/components/accounts/TransactionsList.vue';
  import transactionsBlock from '@/components/accounts/TransactionsBlock.vue';

  const props = defineProps<{
    bankAccountTransactions: BankAccountTransactions[]
  }>();
</script>
