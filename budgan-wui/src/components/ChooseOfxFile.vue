<template>
  <div class="d-flex flex-row align-content-start ma-2">
    <v-card class="file-input pt-4 pr-4 pl-4 pb-4 mr-2">
      <v-file-input
          class="w-100"
          v-model="ofxFileName"
          @update:modelValue="onFileNameUpdated"
          accept=".ofx"
        ></v-file-input>
      <v-col class="text-center">
        <v-btn
            :disabled="!canLoad"
            @click="onLoad">
          Load
        </v-btn>
      </v-col>
    </v-card>
    <v-card class="content ml-2">
      hello world
    </v-card>
  </div>
</template>


<style scoped>

  .file-input {
    flex: 1 1 0;
  }

  .content {
    flex: 2 1 0;
  }

</style>


<script setup lang="ts">

import {computed, inject} from 'vue';
import {type BankAccountTransaction, type BankAccountTransactions, useBankAccountsStore} from '@/stores/bankAccounts';
import type {Container} from 'inversify';
import {ServicesTypes} from '@/services/types';
import type {IOfxParser} from '@/services/ofxParser';
import type {IBankAccountsRepository} from '@/services/BankAccountsRepository';
import type {OfxDocument, OfxTransaction} from '@models/ofxDocument';

const container = inject('container') as Container;
const ofxFileName = defineModel<File[]>();
const canLoad = computed(() => {
  return ofxFileName.value && ofxFileName.value.length > 0
});

const bankAccountsStore = useBankAccountsStore();

let accountsRepository : IBankAccountsRepository | undefined;


function onLoad(event: Event) {

  if (!ofxFileName.value)
    return;


  const reader = new FileReader();

  reader.onload = () => {
    const content : string = reader.result as string;
    const ofxParser : IOfxParser = container.get(ServicesTypes.OfxParser);

    if (ofxParser) {
      const result = ofxParser.parse(content);

      if (result.document) {
        addNewBankAccount(result.document);
      }
    }
  };

  reader.readAsText(ofxFileName.value[0]);
}

function onFileNameUpdated(files: File[]) {

}

function OfxToBankAccountTransaction(ofxTransaction: OfxTransaction) : BankAccountTransaction {
  if (ofxTransaction.fitId == undefined) {
    throw new Error('Transaction ID not found in OFX file.');
  }

  if (ofxTransaction.datePosted == undefined) {
    throw new Error('Transaction date not found in OFX file.');
  }

  if (ofxTransaction.amount == undefined) {
    throw new Error('Transaction amount not found in OFX file.');
  }

  if (ofxTransaction.type == undefined) {
    throw new Error('Transaction type not found in OFX file.');
  }

  return {
    transactionId: ofxTransaction.fitId,
    date: ofxTransaction.datePosted,
    amount: ofxTransaction.amount,
    type: ofxTransaction.type,
    description: ofxTransaction.name || ''
  }
}

function addNewBankAccount(document: OfxDocument) {

  if (document.accountId == undefined) {
    throw new Error('Account ID not found in OFX file.');
  }

  if (document.startDate == undefined) {
    throw new Error('Start date not found in OFX file.');
  }

  if (document.endDate == undefined) {
    throw new Error('End date not found in OFX file.');
  }

  const account = bankAccountsStore.getOrCreateAccount(document.accountId, document.accountType || '');

  if (document.transactions && document.transactions.length > 0) {

    const bankAccountTransactions = document.transactions.map(OfxToBankAccountTransaction);

    bankAccountsStore.addTransactions(account.accountId, document.startDate, document.endDate, bankAccountTransactions);
  }
}

</script>

