import type {BankTransaction} from '@models/BankTransaction';

export interface IBankAccountTransactions {

}

export class BankAccountTransactions implements IBankAccountTransactions {

    dateStart_ : Date | undefined;
    dateEnd_ : Date | undefined;

    transactions_ : Array<BankTransaction> = new Array<BankTransaction>();

    constructor(dateStart: Date | undefined, dateEnd: Date | undefined) {
        this.dateStart_ = dateStart;
        this.dateEnd_ = dateEnd;
    }

    add(transaction: BankTransaction) {
        this.transactions_.push(transaction);
    }
}
