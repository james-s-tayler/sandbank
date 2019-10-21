export interface Transaction {
    id: number;
    amount: number;
    transactionTimeUtc: string;
    transactionType: string;
    merchantName: string;
    description: string;
}
