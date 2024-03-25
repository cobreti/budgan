import type {BankAccount, BankAccountTransaction} from '@models/BankAccountTypes';
import {defineStore} from 'pinia';
import {type Ref, ref} from 'vue';

export type AccountViewTransaction = BankAccountTransaction & {
    groupId: string;
}


export type AccountViewStore = {
    viewTransactions: Ref<AccountViewTransaction[]>;
    account: Ref<BankAccount | undefined>;
    addBankAccount: (bankAccount: BankAccount) => void;
};


export const useAccountViewStore = defineStore<string, AccountViewStore>('accountView',  () => {

    const viewTransactions = ref<AccountViewTransaction[]>([]);
    const account = ref<BankAccount>();

    function addBankAccount(bankAccount: BankAccount) {
        account.value = bankAccount;

        viewTransactions.value = bankAccount.transactions.reduce((acc: AccountViewTransaction[], group) => {
            const groupId = group.id;

            const viewTransactions = group.transactions.reduce((tacc: AccountViewTransaction[], transaction) => {
                return [...tacc, {...transaction, groupId}];
            }, []);

            return [...acc, ...viewTransactions];
        }, []);
    }

    return {
        viewTransactions,
        account,
        addBankAccount
    };
});
