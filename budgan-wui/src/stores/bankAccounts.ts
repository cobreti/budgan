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

export type BankAccountsStore = {
    accounts: Ref<{[key: string]: BankAccount}>
    getAccountById: (accountId: string) => BankAccount;
    createAccount: (accountId: string) => BankAccount;
}

export const useBankAccountsStore = defineStore<string, BankAccountsStore>('bankTransactions',  () => {
    const name = ref("test");
    const accounts = ref({
        '1234': {
            accountId: '1234',
            accountType: 'checking',
            transactions: [
                {
                    dateStart: new Date(),
                    dateEnd: new Date(),
                    transactions: [
                        {
                            transactionId: '1',
                            date: new Date(),
                            amount: 100,
                            description: 'Initial deposit'
                        }
                    ]
                }
            ]
        }
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

    return {
        accounts,
        getAccountById,
        createAccount
    }
});
