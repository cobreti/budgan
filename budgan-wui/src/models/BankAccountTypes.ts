export declare type TransactionIdsTable = {[key: string]: undefined};

export type BankAccountTransaction = {
    transactionId: string;
    date: Date;
    amount: number;
    type: string;
    description: string;
}

export type BankAccountTransactionsGroup = {

    dateStart: Date;
    dateEnd: Date;
    transactions: BankAccountTransaction[];
}

export type BankAccount = {
    accountId: string;
    accountType: string | undefined;
    transactionsId: TransactionIdsTable
    transactions: BankAccountTransactionsGroup[];
}