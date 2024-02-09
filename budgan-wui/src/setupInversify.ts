import 'reflect-metadata';
import {Container} from 'inversify';
import {ServicesTypes} from '@/services/types';
import {OfxParser} from '@/services/ofxParser';
import type {IBankAccountsRepository} from '@/services/BankAccountsRepository';
import {BankAccountsRepository} from '@/services/BankAccountsRepository';

const container = new Container();

container.bind(ServicesTypes.OfxParser).to(OfxParser);
container.bind<IBankAccountsRepository>(ServicesTypes.BankAccountsRepository).to(BankAccountsRepository).inSingletonScope();

export {
    container
}
