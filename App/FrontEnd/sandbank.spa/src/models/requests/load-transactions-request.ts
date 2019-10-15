import { Account } from '@/account';

export interface LoadTransactionsRequest {
    account: Account;
    range: string[]; // iso date format for start/end dates
}
