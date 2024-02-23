import { defineStore } from 'pinia'
import {type Ref, ref} from 'vue';


export type BankAccountTransaction = {
    transactionId: string;
    date: Date;
    amount: number;
    type: string;
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
    transactionsId: Set<string>;
    transactions: BankAccountTransactions[];
}

export type BankAccountTable = {[key: string]: BankAccount};


export type BankAccountsStore = {
    accounts: Ref<BankAccountTable>
    getAccountById: (accountId: string) => BankAccount;
    createAccount: (accountId: string,  accountType: string) => BankAccount;
    getOrCreateAccount: (accountId: string, accountType: string) => BankAccount;
    addTransactions: (accountId: string, startDate: Date, endDate: Date, transactions: BankAccountTransaction[]) => void;
}

export const useBankAccountsStore = defineStore<string, BankAccountsStore>('bankTransactions',  () => {

    const accounts = ref<BankAccountTable>({});

    function getAccountById(accountId: string) {
            return accounts.value[accountId];
    }

    function createAccount(accountId: string, accountType: string) {

        const account: BankAccount = {
            accountId: accountId,
            accountType: accountType,
            transactionsId: new Set<string>(),
            transactions: []
        }
        accounts.value[accountId] = account;

        return account;
    }

    function getOrCreateAccount(accountId: string, accountType: string) {
        let account = getAccountById(accountId);
        if (!account) {
            account = createAccount(accountId, accountType);
        }
        return account;
    }

    function addTransactions(accountId: string, startDate: Date, endDate: Date, transactions: BankAccountTransaction[]) {
        const account = getAccountById(accountId);

        if (!account) {
            throw new Error('Account not found');
        }

        const existingBankAccountTransactions = account.transactions.find((transaction) => {
            return transaction.dateStart === startDate && transaction.dateEnd === endDate;
        });

        if (existingBankAccountTransactions) {
            throw new Error("Transactions already exist for this date range")
        }

        const newTransactions = transactions.filter((transaction) => {
            return !account.transactionsId.has(transaction.transactionId);
        });

        if (newTransactions.length > 0) {
            newTransactions.forEach((transaction) => {
                account.transactionsId.add(transaction.transactionId);
            });

            const bankAccountTransactions: BankAccountTransactions = {
                dateStart: startDate,
                dateEnd: endDate,
                transactions: newTransactions
            }

            account.transactions.push(bankAccountTransactions);
        }
    }

    return {
        accounts,
        getAccountById,
        createAccount,
        getOrCreateAccount,
        addTransactions
    }
});
