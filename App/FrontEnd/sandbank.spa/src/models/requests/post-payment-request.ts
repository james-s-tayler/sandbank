export interface PostPaymentRequest {
    fromAccount: string;
    toAccount: string;
    amount: number;
    description: string;
    merchantName: string;
}
