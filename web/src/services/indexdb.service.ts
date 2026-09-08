import Dexie, { EntityTable, Table } from 'dexie';
import { Injectable } from '@angular/core';
import { JournalModel } from '@models/journalModel';
import { ColumnsMapping } from '@models/columnsMappingModel';
import { AccountModel } from '@models/accountModel';
import { fileModel } from '@models/fileModel';
import { AccountTransactionModel } from '@models/accountTransactionModel';
import { AccountRecurringTransactionModel } from '@models/accountRecurringTransactionModel';
import { AllDataExportPayload } from './budgan-export.service';

export type WorkspaceEntity = EntityTable<JournalModel, 'id'>;

@Injectable({
  providedIn: 'root',
})
export class IndexdbService extends Dexie {
  workspaceTable!: WorkspaceEntity;
  columnsMappingTable!: Table<ColumnsMapping, string>;
  accountsTable!: EntityTable<AccountModel, 'id'>;
  filesTable!: EntityTable<fileModel, 'id'>;
  accountTransactionsTable!: EntityTable<AccountTransactionModel, 'id'>;
  recurringTransactionsTable!: EntityTable<
    AccountRecurringTransactionModel,
    'id'
  >;

  constructor() {
    super('budgan');
    this.version(1).stores({
      workspaces: '&id, &name',
      columnMappings: '&id, &name',
      accounts: '&id, &name',
      files: '&id, filename, accountId',
      accountTransactions:
        '&id, accountId, fileId, &[accountId+uniqueKey], *dateInscriptionAsString',
      accountRecurringTransactions: null,
      recurringTransactions: '&id, accountId',
    });

    this.workspaceTable = this.table('workspaces');
    this.columnsMappingTable = this.table('columnMappings');
    this.accountsTable = this.table('accounts');
    this.filesTable = this.table('files');
    this.accountTransactionsTable = this.table('accountTransactions');
    this.recurringTransactionsTable = this.table('recurringTransactions');

    this.open();
  }

  async clearAll(): Promise<void> {
    await this.transaction(
      'rw',
      [
        this.workspaceTable,
        this.columnsMappingTable,
        this.accountsTable,
        this.filesTable,
        this.accountTransactionsTable,
        this.recurringTransactionsTable,
      ],
      async () => {
        await Promise.all([
          this.workspaceTable.clear(),
          this.columnsMappingTable.clear(),
          this.accountsTable.clear(),
          this.filesTable.clear(),
          this.accountTransactionsTable.clear(),
          this.recurringTransactionsTable.clear(),
        ]);
      }
    );
  }

  async replaceAll(payload: AllDataExportPayload): Promise<void> {
    await this.transaction(
      'rw',
      [
        this.workspaceTable,
        this.columnsMappingTable,
        this.accountsTable,
        this.filesTable,
        this.accountTransactionsTable,
        this.recurringTransactionsTable,
      ],
      async () => {
        await Promise.all([
          this.workspaceTable.clear(),
          this.columnsMappingTable.clear(),
          this.accountsTable.clear(),
          this.filesTable.clear(),
          this.accountTransactionsTable.clear(),
          this.recurringTransactionsTable.clear(),
        ]);
        await Promise.all([
          this.columnsMappingTable.bulkAdd(payload.columnsMappings),
          this.accountsTable.bulkAdd(payload.accounts),
          this.filesTable.bulkAdd(payload.files),
          this.accountTransactionsTable.bulkAdd(payload.transactions),
          ...(payload.recurringTransactions?.length
            ? [
                this.recurringTransactionsTable.bulkAdd(
                  payload.recurringTransactions
                ),
              ]
            : []),
        ]);
      }
    );
  }
}
