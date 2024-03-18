import { defineStore } from 'pinia'
import {type Ref, ref} from 'vue';
import type {
    BankAccount, BankAccountsDictionary,
    BankAccountTransaction,
    BankAccountTransactionsGroup,
    TransactionIdsTable
} from '@models/BankAccountTypes';


export type BankAccountsStore = {
    accounts: Ref<BankAccountsDictionary>
    getAccountById: (accountId: string) => BankAccount;
    addWithBankAccount: (account: BankAccount) => void;
}

export const useBankAccountsStore = defineStore<string, BankAccountsStore>('bankTransactions',  () => {

    const accounts = ref<BankAccountsDictionary>({});

    function getAccountById(accountId: string) {
            return accounts.value[accountId];
    }

    function sanitizeTransactionsGroup(destAccount: BankAccount, transactionsGroup: BankAccountTransactionsGroup) {

        const newTransactions = transactionsGroup.transactions.filter((transaction) => {
            return !(transaction.transactionId in destAccount.transactionsId);
        });

        return {
            dateStart: transactionsGroup.dateStart,
            dateEnd: transactionsGroup.dateEnd,
            transactions: newTransactions
        } as BankAccountTransactionsGroup;
    }

    function addWithBankAccount(account: BankAccount) {

        const existingAccount = getAccountById(account.accountId);
        if (!existingAccount) {
            accounts.value[account.accountId] = account;
            return;
        }

        const sanitizedTransactionsGroup = account.transactions.map((transactionsGroup) => {
                return sanitizeTransactionsGroup(existingAccount, transactionsGroup);
            })
            .filter((transactionsGroup) => transactionsGroup.transactions.length > 0);

        if (sanitizedTransactionsGroup.length > 0) {
            const newIds = sanitizedTransactionsGroup
                .reduce((acc: TransactionIdsTable, transactionGroup) => {
                    const ids = transactionGroup.transactions.map(transaction => transaction.transactionId)
                        .reduce((idsAcc:TransactionIdsTable, id) => {
                            idsAcc[id] = undefined;
                            return idsAcc;
                        }, {});
                    return {...acc, ...ids};
                }, {});

            existingAccount.transactionsId = {...existingAccount.transactionsId, ...newIds};
            existingAccount.transactions = [...existingAccount.transactions, ...sanitizedTransactionsGroup];
        }
    }

    return {
        accounts,
        getAccountById,
        addWithBankAccount
    }
}, {
    persist: {
        storage: sessionStorage,
        afterRestore(context) {
            const accounts = context.store.accounts;

            for (const accountId in accounts) {
                const account = accounts[accountId];
                account.transactions = account.transactions.map((transactionsGroup : BankAccountTransactionsGroup) => {
                    return {
                        dateStart: new Date(transactionsGroup.dateStart),
                        dateEnd: new Date(transactionsGroup.dateEnd),
                        transactions: transactionsGroup.transactions.map((transaction :BankAccountTransaction) => {
                            return {
                                ...transaction,
                                date: new Date(transaction.date)
                            };
                        })
                    };
                });
            }
        }
    }
});
