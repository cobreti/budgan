import { defineStore } from 'pinia'
import {type Ref, ref} from 'vue';


export type BankAccountTransaction = {
    transactionId: string;
    date: Date;
    amount: number;
    description: string;
}

export type BankAccountTransactions = {

    dateStart: Date;
    dateEnd: Date;
    transactions: BankAccountTransaction[];
}

export type BankAccount = {
    accountId: string;
    accountType: string | undefined;
    transactions: BankAccountTransactions[];
}

export type BankAccountTable = {[key: string]: BankAccount};


export type BankAccountsStore = {
    accounts: Ref<BankAccountTable>
    getAccountById: (accountId: string) => BankAccount;
    createAccount: (accountId: string) => BankAccount;
    getOrCreateAccountById: (accountId: string) => BankAccount;
}

export const useBankAccountsStore = defineStore<string, BankAccountsStore>('bankTransactions',  () => {
    const name = ref("test");
    const accounts = ref<BankAccountTable>({
    });

    function getAccountById(accountId: string) {
            return accounts.value[accountId];
    }

    function createAccount(accountId: string) {
        const account: BankAccount = {
            accountId: accountId,
            accountType: undefined,
            transactions: []
        }
        accounts.value[accountId] = account;

        return account;
    }

    function getOrCreateAccountById(accountId: string) {
        let account = getAccountById(accountId);
        if (!account) {
            account = createAccount(accountId);
        }
        return account;
    }

    return {
        accounts,
        getAccountById,
        createAccount,
        getOrCreateAccountById
    }
});
