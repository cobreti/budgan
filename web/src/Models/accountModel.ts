export type AccountType = 'credit' | 'debit';

export interface AccountModel {
  id: string;
  name: string;
  columnsMappingId: string;
  accountType?: AccountType;
  timestamp?: string;
}
