import { inject, Injectable, Signal, signal } from '@angular/core';
import Dexie from 'dexie';
import { IndexdbService } from '@services/indexdb.service';
import {
  AccountTransactionModel,
  AccountTransactionRecordType,
  buildSnapshotUniqueKey,
  buildTransactionUniqueKey,
} from '@models/accountTransactionModel';
import { Result } from '@app-types/result';
import { ID_GENERATOR_SERVICE, IdGeneratorService } from '@services/id-generator.service';
import {
  AccountTransactionService,
  TransactionPage,
  TransactionSort,
} from '@services/account-transaction.service';

@Injectable({ providedIn: 'root' })
export class AccountTransactionServicePwaImpl implements AccountTransactionService {
  private readonly _indexDb = inject(IndexdbService);
  private readonly _transactionsVersion = signal(0);
  private readonly _idGenerator = inject<IdGeneratorService>(ID_GENERATOR_SERVICE);

  readonly transactionsVersion: Signal<number> = this._transactionsVersion;

  async getList(): Promise<AccountTransactionModel[]> {
    return this._indexDb.accountTransactionsTable.toArray();
  }

  async getCountByAccount(accountId: string): Promise<number> {
    return this._indexDb.accountTransactionsTable.where('accountId').equals(accountId).count();
  }

  async getPageByAccount(
    accountId: string,
    page: number,
    pageSize: number,
    sort: TransactionSort,
  ): Promise<TransactionPage> {
    const all = await this._indexDb.accountTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();

    const sorted = this._sortTransactions(all, sort);
    const totalPages = Math.ceil(sorted.length / pageSize);
    const transactions = sorted.slice(page * pageSize, (page + 1) * pageSize);

    return { transactions, totalPages, page };
  }

  private _sortTransactions(
    transactions: AccountTransactionModel[],
    sort: TransactionSort,
  ): AccountTransactionModel[] {
    const sign = sort.direction === 'desc' ? -1 : 1;
    return [...transactions].sort((a, b) => {
      switch (sort.field) {
        case 'amount':
          return sign * (a.amount - b.amount);
        case 'cardNumber':
          return sign * a.cardNumber.localeCompare(b.cardNumber);
        case 'description':
          return sign * a.description.localeCompare(b.description);
        case 'dateInscription': {
          const dateCmp = a.dateInscriptionAsString.localeCompare(b.dateInscriptionAsString);
          if (dateCmp !== 0) return sign * dateCmp;
          const aOff = a.balanceDateOffset ?? 0;
          const bOff = b.balanceDateOffset ?? 0;
          return sign * (aOff - bOff);
        }
      }
    });
  }

  async create(
    fileId: string,
    accountId: string,
    cardNumber: string,
    dateInscriptionAsString: string,
    amount: number,
    description: string,
  ): Promise<Result<string>> {
    const uniqueKey = buildTransactionUniqueKey(
      accountId,
      cardNumber,
      dateInscriptionAsString,
      amount,
      description,
    );
    const id = this._idGenerator.generateId();
    const range = 0.15;
    const lower = Math.floor((amount * (1 - range)) / 5) * 5;
    const upper = Math.floor((amount * (1 + range)) / 5) * 5;
    const recurringId = `${accountId}|${cardNumber}|${lower}|${upper}|${description}`;
    try {
      await this._indexDb.accountTransactionsTable.add({
        id,
        uniqueKey,
        recurringId,
        fileId,
        accountId,
        cardNumber,
        dateInscriptionAsString,
        amount,
        description,
        recordType: AccountTransactionRecordType.normal,
      });
      return { success: true, value: id };
    } catch (e) {
      if (e instanceof Dexie.ConstraintError) {
        return { success: false, error: 'duplicate-transaction' };
      }
      throw e;
    }
  }

  private snapshotId(accountId: string): string {
    return buildSnapshotUniqueKey(accountId);
  }

  async setSnapshot(
    accountId: string,
    dateAsString: string,
    amount: number,
  ): Promise<Result<string>> {
    const uniqueKey = this.snapshotId(accountId);
    const existing = await this._indexDb.accountTransactionsTable
      .where('[accountId+uniqueKey]')
      .equals([accountId, uniqueKey])
      .first();
    const id = existing?.id ?? this._idGenerator.generateId();
    await this._indexDb.accountTransactionsTable.put({
      id,
      uniqueKey,
      recurringId: uniqueKey,
      fileId: '',
      accountId,
      cardNumber: '',
      dateInscriptionAsString: dateAsString,
      amount,
      balance: amount,
      balanceDateOffset: 0,
      description: '',
      recordType: AccountTransactionRecordType.snapshot,
    });
    await this.recalculateBalances(accountId);
    return { success: true, value: id };
  }

  async getSnapshot(accountId: string): Promise<AccountTransactionModel | undefined> {
    const uniqueKey = this.snapshotId(accountId);
    return this._indexDb.accountTransactionsTable
      .where('[accountId+uniqueKey]')
      .equals([accountId, uniqueKey])
      .first();
  }

  async deleteSnapshot(accountId: string): Promise<void> {
    const uniqueKey = this.snapshotId(accountId);
    await this._indexDb.accountTransactionsTable
      .where('[accountId+uniqueKey]')
      .equals([accountId, uniqueKey])
      .delete();
    await this.recalculateBalances(accountId);
  }

  async getListByAccount(accountId: string): Promise<AccountTransactionModel[]> {
    return this._indexDb.accountTransactionsTable.where('accountId').equals(accountId).toArray();
  }

  async getById(id: string): Promise<AccountTransactionModel> {
    const entry = await this._indexDb.accountTransactionsTable.get(id);
    if (!entry) {
      throw new Error('Account transaction not found');
    }
    return entry;
  }

  async delete(id: string): Promise<void> {
    await this._indexDb.accountTransactionsTable.delete(id);
  }

  /**
   * Recomputes the running `balance` and `balanceDateOffset` for every transaction
   * of an account, anchored on the user-entered snapshot.
   *
   * The snapshot is the one known-true balance at a given date. From it we walk:
   *   - forward  (dates >= snapshot) adding each amount, and
   *   - backward (dates <  snapshot) subtracting each amount,
   * so every row gets the account balance as of that row. `balanceDateOffset` records
   * each row's order relative to the snapshot (0) to break ties when several rows share
   * the same date. With no snapshot, the opening balance is assumed to be 0 and balances
   * run forward from there.
   */
  async recalculateBalances(accountId: string): Promise<void> {
    // Load every record for this account: the (optional) snapshot plus normal rows.
    const all = await this._indexDb.accountTransactionsTable
      .where('accountId')
      .equals(accountId)
      .toArray();

    // The snapshot is the anchor; there is at most one per account.
    const snapshot = all.find((t) => t.recordType === AccountTransactionRecordType.snapshot);

    // Normal rows in chronological order; uniqueKey is a stable tiebreaker for equal dates.
    const normal = all
      .filter((t) => t.recordType === AccountTransactionRecordType.normal)
      .sort((a, b) => {
        const dateCmp = a.dateInscriptionAsString.localeCompare(b.dateInscriptionAsString);
        if (dateCmp !== 0) return dateCmp;
        return a.uniqueKey.localeCompare(b.uniqueKey);
      });

    // No snapshot -> assume an opening balance of 0 the day before the earliest row
    // and run balances forward from there, so the data is still meaningful.
    if (!snapshot) {
      if (normal.length === 0) {
        this._transactionsVersion.update((v) => v + 1);
        return;
      }

      // Forward pass from 0: assign each row the running balance after adding its amount.
      // Offset increases (1, 2, ...) to keep a stable order for rows sharing a date.
      let running = 0;
      let offset = 0;
      const updated = normal.map((t) => {
        offset += 1;
        running += t.amount;
        return { ...t, balance: running, balanceDateOffset: offset };
      });
      await this._indexDb.accountTransactionsTable.bulkPut(updated);

      this._transactionsVersion.update((v) => v + 1);
      return;
    }

    // Partition normal rows around the snapshot date.
    // `afterOrEqual` includes the snapshot date itself (forward pass);
    // `before` is everything strictly earlier (backward pass).
    const snapshotDate = snapshot.dateInscriptionAsString;
    const before = normal.filter((t) => t.dateInscriptionAsString < snapshotDate);
    const afterOrEqual = normal.filter((t) => t.dateInscriptionAsString >= snapshotDate);

    // Forward pass: start at the snapshot balance and ADD each later amount.
    // Offset increases (+1, +2, ...) the further a row is after the snapshot.
    let running = snapshot.amount;
    let offset = 0;
    const updatedAfter = afterOrEqual.map((t) => {
      offset += 1;
      running += t.amount;
      return { ...t, balance: running, balanceDateOffset: offset };
    });

    // Backward pass: reset to the snapshot balance and walk earlier rows newest->oldest,
    // assigning each row's balance BEFORE subtracting its amount (so each row stores the
    // balance as of itself). Offset decreases (-1, -2, ...). unshift keeps order ascending.
    running = snapshot.amount;
    offset = 0;
    const updatedBefore: AccountTransactionModel[] = [];
    for (let i = before.length - 1; i >= 0; i--) {
      offset -= 1;
      const t = before[i];
      updatedBefore.unshift({ ...t, balance: running, balanceDateOffset: offset });
      running -= t.amount;
    }

    // `ordered` is fully chronological (before rows, then after-or-equal rows). Persist all.
    const ordered = [...updatedBefore, ...updatedAfter];
    await this._indexDb.accountTransactionsTable.bulkPut(ordered);

    // Bump the version signal so dependent views recompute.
    this._transactionsVersion.update((v) => v + 1);
  }
}
