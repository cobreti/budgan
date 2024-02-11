<template>
  <div>
    <v-card class="pt-4 pr-4 pl-4 pb-4 mb-8">
      <v-file-input
          v-model="ofxFileName"
          @update:modelValue="onFileNameUpdated"
          accept=".ofx"
        ></v-file-input>
    </v-card>
    <v-col class="text-center">
      <v-btn
          :disabled="!canLoad"
          @click="onLoad"
        >
        Load
      </v-btn>
    </v-col>
  </div>
</template>


<style scoped>

</style>


<script setup lang="ts">

import {computed, inject} from 'vue';
import { useBankAccountsStore } from '@/stores/bankAccounts';
import type {Container} from 'inversify';
import {ServicesTypes} from '@/services/types';
import type {IOfxParser} from '@/services/ofxParser';
import type {IBankAccountsRepository} from '@/services/BankAccountsRepository';
import type {OfxDocument} from '@models/ofxDocument';
import {BankAccountTransactions, type IBankAccountTransactions} from '@models/BankAccountTransactions';

const container = inject('container') as Container;
const ofxFileName = defineModel<File[]>();
const canLoad = computed(() => {
  return ofxFileName.value && ofxFileName.value.length > 0
});

const bankAccountsStore = useBankAccountsStore();
const { getAccountById, createAccount } = bankAccountsStore;

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

function getOrCreateBankAccount(accountId: string) : BankAccount {


  const account = getAccountById(accountId);
  if (account) {
    return account;
  }

  return createAccount(accountId);
}

function addNewBankAccount(document: OfxDocument) {

  if (document.accountId == undefined) {
    throw new Error('Account ID not found in OFX file.');
  }

  const account = getOrCreateBankAccount(document.accountId);


  // if (!accountsRepository) {
  //   accountsRepository = container.get(ServicesTypes.BankAccountsRepository) as IBankAccountsRepository;
  //
  //   if (!accountsRepository) {
  //     throw new Error('BankAccountsRepository not found in container.');
  //   }
  // }
  //
  //
  // const account = accountsRepository.getOrCreateAccount(document.accountId);
  // createAccountTransactions(document);
}

function createAccountTransactions(document: OfxDocument) {
  // const accountTransactions = new BankAccountTransactions(document.startDate, document.endDate);
  //
  // document.transactions.forEach(transaction => {
  //   accountTransactions.add(transaction);
  // });
}

</script>

