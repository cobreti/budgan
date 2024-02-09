import type {IBankAccount} from '@models/BankAccount';
import {injectable} from 'inversify';
import {BankAccount} from '@models/BankAccount';

export interface IBankAccountsRepository {
    getOrCreateAccount(accountId: string) : IBankAccount;
}

@injectable()
export class BankAccountsRepository implements IBankAccountsRepository {

    accounts_ : { [key: string]: IBankAccount } = {};

    getOrCreateAccount(accountId: string) : IBankAccount {
        let account = this.accounts_[accountId];
        if (!account) {
            account = this.createAccount(accountId);
        }
        return account;
    }

    createAccount(accountId: string) : IBankAccount {
        let account = new BankAccount(accountId);
        this.accounts_[accountId] = account;
        return account;
    }
}
