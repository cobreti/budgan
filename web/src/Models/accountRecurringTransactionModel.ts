export interface AccountRecurringTransactionModel {
  id: string; // equals AccountTransactionModel.recurringId
  accountId: string;
  periodInDays: number;
  transactionCount: number;
  description: string;
  averageAmount: number;
  firstOccurrenceDate: string;
  lastOccurrenceDate: string;
}
