import { Transaction } from './transaction';

export interface Account {
    accountType: string;
    accountNumber: string;
    displayName: string;
    accountOwnerId: number;
    balance: number;
    transactions: Transaction[];
    id: number;
}
