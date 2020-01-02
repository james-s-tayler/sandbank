import { Transaction } from './transaction';

export interface Account {
    accountType: string;
    accountNumber: string;
    accountOwnerId: number;
    balance: number;
    transactions: Transaction[];
    displayName: string;
    imageUrl: string;
    id: number;
}
