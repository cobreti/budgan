import { AccountModel } from '@/Models/accountModel';
import { AccountTransactionModel } from '@/Models/accountTransactionModel';
import { ACCOUNT_SERVICE, AccountService } from '@/services/account.service';
import { CommonModule } from '@angular/common';
import {
  Component,
  effect,
  inject,
  input,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';

export type AccountTransactionsViewModel = {
  account: string;
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
export class PiechartTransactionsTableComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;

  readonly accountService = inject<AccountService>(ACCOUNT_SERVICE);
  readonly showAccountColumn = input<boolean>(false);
  readonly accountTransactions = input.required<AccountTransactionModel[]>();
  readonly color = input<string | undefined>('');

  accounts = signal<AccountModel[]>([]);
  readonly allColumns: string[] = [
    'account',
    'description',
    'dateInscription',
    'amount',
  ];
  displayedColumns: string[] = [];

  dataSource = new MatTableDataSource<AccountTransactionsViewModel>();

  constructor() {
    effect(() => {
      const showAccountColumn = this.showAccountColumn();
      (this, this.updateDisplayedColumns(showAccountColumn));
    });

    effect(() => {
      const trxs = this.accountTransactions();
      const accounts = this.accounts();
      this.updateDataSource(accounts, trxs);
    });
  }

  async ngOnInit(): Promise<void> {
    const accounts = await this.accountService.getList();
    this.accounts.set(accounts);

    this.updateDisplayedColumns(this.showAccountColumn());
  }

  updateDataSource(
    accounts: AccountModel[],
    transactions: AccountTransactionModel[]
  ) {
    this.dataSource.data = transactions.map((trx) => ({
      account: accounts.find((x) => x.id == trx.accountId)?.name ?? '',
      description: trx.description,
      amount: trx.amount,
      dateInscription: trx.dateInscriptionAsString,
    }));
  }

  updateDisplayedColumns(showAccountColumn: boolean) {
    if (!showAccountColumn) {
      this.displayedColumns = this.allColumns.filter((x) => x != 'account');
    } else {
      this.displayedColumns = this.allColumns;
    }
  }
}
