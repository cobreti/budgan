import {defineStore} from 'pinia';
import {ref, type Ref} from 'vue';
import type {BankAccount} from '@models/BankAccountTypes';
import {timer} from 'rxjs';
import {resolve} from 'inversify/lib/resolution/resolver';

export declare type LoadedAccount = {
    loading: boolean,
    account: BankAccount | undefined;
}

export type AddStatementStore = {
    loadedAccount: Ref<LoadedAccount>;
    loadOfxFile: (file: File) => void;
};

export const useAddStatementStore = defineStore<string, AddStatementStore>('addStatement',  () => {

    const loadedAccount = ref<LoadedAccount>({
        loading: false,
        account: undefined
    });

    function loadOfxFile(file: File) : void {
        loadedAccount.value = {
            loading: true,
            account: undefined
        };

        timer(5000).subscribe(() => {
            loadedAccount.value = {
                loading: false,
                account: {
                    accountId: '123',
                    accountType: 'checking',
                    transactionsId: {},
                    transactions: []
                }
            };
        });

    }

    return {
        loadedAccount,
        loadOfxFile
    }
});
