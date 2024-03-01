<template>

  <div class="transactions-list-panel">
    <ul>
      <li v-for="transaction in transactions" :key="transaction.id">
        {{transaction.date}} - {{transaction.amount}}
      </li>
    </ul>
  </div>
</template>


<style scoped>
  .transactions-list-panel {
    position: relative;
    display: block;
    //padding-left: 1em;
    //max-height: 8em;
    //overflow: auto;

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
import type {BankAccountTransaction, BankAccountTransactions} from '@/stores/bankAccounts';

const props = defineProps<{
  transactions: BankAccountTransaction[]
}>();
</script>
