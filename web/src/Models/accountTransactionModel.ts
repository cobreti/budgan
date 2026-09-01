export interface AccountTransactionModel {
  id: string;
  uniqueKey: string;
  recurringId: string;
  fileId: string;
  accountId: string;
  cardNumber: string;
  dateInscriptionAsString: string;
  amount: number;
  balance?: number;
  balanceDateOffset?: number;
  description: string;
  recordType: AccountTransactionRecordType;
}

export enum AccountTransactionRecordType {
  normal = 'normal',
  snapshot = 'snapshot',
}

export function buildTransactionUniqueKey(
  accountId: string,
  cardNumber: string,
  dateInscriptionAsString: string,
  amount: number,
  description: string,
): string {
  return `${accountId}|${dateInscriptionAsString}|${amount}|${description}`;
}

export function buildSnapshotUniqueKey(accountId: string): string {
  return `snapshot|${accountId}`;
}
