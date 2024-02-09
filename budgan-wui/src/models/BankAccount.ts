import type {IBankAccountTransactions} from '@models/BankAccountTransactions';

export interface IBankAccount {

    accountId: string;
    accountType: string | undefined;


}

export class BankAccount implements IBankAccount {

    accountId_ : string;
    accountType_ : string | undefined = undefined
    transactions: { [key: string]: IBankAccountTransactions;} = {}

    constructor(accountId: string) {
        this.accountId_ = accountId;
    }

    get accountId() {
        return this.accountId_;
    }

    get accountType() : string | undefined {
        return this.accountType_;
    }

    set accountType(accountType: string) {
        this.accountType_ = accountType;
    }
}
