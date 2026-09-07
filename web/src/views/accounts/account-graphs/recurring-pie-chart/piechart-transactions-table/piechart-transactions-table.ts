import { AccountModel } from '@/Models/accountModel';
import { AccountTransactionModel } from '@/Models/accountTransactionModel';
import { ACCOUNT_SERVICE, AccountService } from '@/services/account.service';
import { CommonModule } from '@angular/common';
import {
  Component,
  effect,
  EventEmitter,
  inject,
  input,
  OnInit,
  Output,
  signal,
  ViewChild,
} from '@angular/core';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';
import { MatIcon } from '@angular/material/icon';
import { MatIconButton } from '@angular/material/button';
import { EventHandler } from '@azure/msal-browser';

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
  imports: [
    TranslatePipe,
    MatTableModule,
    MatSortModule,
    CommonModule,
    MatIconButton,
    MatIcon,
  ],
})
export class PiechartTransactionsTableComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;

  @Output() readonly close = new EventEmitter<void>();

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

  onClose() {
    this.close.emit();
  }
}
