import { AccountTransactionModel } from '@/Models/accountTransactionModel';
import { CommonModule } from '@angular/common';
import { Component, effect, input, ViewChild } from '@angular/core';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';

export type AccountTransactionsViewModel = {
  description: string;
  amount: number;
  dateInscription: string;
};

@Component({
  selector: 'app-piechart-transactions-table',
  templateUrl: './piechart-transactions-table.html',
  styleUrl: './piechart-transactions-table.scss',
  imports: [TranslatePipe, MatTableModule, MatSortModule, CommonModule],
})
export class PiechartTransactionsTableComponent {
  @ViewChild(MatSort) sort!: MatSort;

  readonly accountTransactions = input.required<AccountTransactionModel[]>();
  readonly color = input<string | undefined>('');

  displayedColumns = ['description', 'dateInscription', 'amount'];

  dataSource = new MatTableDataSource<AccountTransactionsViewModel>();

  constructor() {
    effect(() => {
      const trxs = this.accountTransactions();
      this.updateDataSource(trxs);
    });
  }

  updateDataSource(transactions: AccountTransactionModel[]) {
    this.dataSource.data = transactions.map((trx) => ({
      description: trx.description,
      amount: trx.amount,
      dateInscription: trx.dateInscriptionAsString,
    }));
  }
}
