import {defineStore} from 'pinia';
import {ref, type Ref} from 'vue';
import type {BankAccount, BankAccountTransaction, TransactionIdsTable} from '@models/BankAccountTypes';
import type {IOfxParser} from '@/services/ofxParser';
import {ServicesTypes} from '@/services/types';
import {container} from '@/setupInversify';
import type {OfxDocument, OfxTransaction} from '@models/ofxDocument';

export declare type LoadedAccount = {
    loading: boolean,
    account: BankAccount | undefined;
    filename: string | undefined;
}

export type AddStatementStore = {
    loadedAccount: Ref<LoadedAccount>;
    loadOfxFile: (file: File) => void;
    clear: () => void;
};

export const useAddStatementStore = defineStore<string, AddStatementStore>('addStatement',  () => {

    const loadedAccount = ref<LoadedAccount>({
        loading: false,
        account: undefined,
        filename: undefined
    });

    function loadOfxFile(file: File) : void {
        loadedAccount.value = {
            loading: true,
            account: undefined,
            filename: file.name
        };

        const reader = new FileReader();

        reader.onload = () => onOfxLoaded(reader.result as string);

        reader.readAsText(file);
    }

    function clear() {
        loadedAccount.value = {
            loading: false,
            account: undefined,
            filename: undefined
        };
    }

    function onOfxLoaded(content: string) {
        const ofxParser : IOfxParser = container.get(ServicesTypes.OfxParser);

        if (ofxParser) {
            const result = ofxParser.parse(content);

            if (result.document) {
                loadAccount(result.document);
            }
        }
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

    function loadAccount(document: OfxDocument) {
        if (document.accountId == undefined) {
            throw new Error('Account ID not found in OFX file.');
        }

        if (document.startDate == undefined) {
            throw new Error('Start date not found in OFX file.');
        }

        if (document.endDate == undefined) {
            throw new Error('End date not found in OFX file.');
        }

        if (document.transactions && document.transactions.length > 0) {

            const bankAccountTransactions = document.transactions.map(OfxToBankAccountTransaction);

            const account : BankAccount = {
                accountId: document.accountId,
                accountType: document.accountType || '',
                transactionsId: bankAccountTransactions.map(transaction => transaction.transactionId)
                    .reduce((acc: TransactionIdsTable, id) => {
                        acc[id] = {};
                        return acc;
                    }, {}),
                transactions: [{
                    id: crypto.randomUUID(),
                    dateStart: document.startDate,
                    dateEnd: document.endDate,
                    transactions: bankAccountTransactions
                }]
            }

            loadedAccount.value = {
                loading: false,
                account: account,
                filename: loadedAccount.value.filename
            }
        }
    }

    return {
        loadedAccount,
        loadOfxFile,
        clear
    }
});
