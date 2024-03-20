import type {BankAccount, BankAccountTransaction} from '@models/BankAccountTypes';
import {defineStore} from 'pinia';
import {type Ref, ref} from 'vue';

export type AccountViewTransaction = BankAccountTransaction & {
    groupId: string;
}


export type AccountViewStore = {
    transactions: Ref<AccountViewTransaction[]>;
    account: Ref<BankAccount | undefined>;
    addBankAccount: (bankAccount: BankAccount) => void;
};

export const useAccountViewStore = defineStore<string, AccountViewStore>('accountView',  () => {

    const transactions = ref<AccountViewTransaction[]>([]);
    const account = ref<BankAccount>();

    function addBankAccount(bankAccount: BankAccount) {
        account.value = bankAccount;

        transactions.value = bankAccount.transactions.reduce((acc: AccountViewTransaction[], group) => {
            const groupId = group.id;

            const viewTransactions = group.transactions.reduce((tacc: AccountViewTransaction[], transaction) => {
                return [...tacc, {...transaction, groupId}];
            }, []);

            return [...acc, ...viewTransactions];
        }, []);
    }

    return {
        transactions,
        account,
        addBankAccount
    };
});
