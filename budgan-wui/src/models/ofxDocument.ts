export type OfxDocument = {
    version: string;
    security: string;
    encoding: string;
    charset: string;
    compression: string;

    currency?: string;
    startDate?: Date;
    endDate?: Date;

    accountType?: string;
    accountId?: string;

    transactions: OfxTransaction[];
};


export type OfxTransaction = {
    type?: string;
    datePosted?: Date;
    amount?: number;
    fitId?: string;
    name?: string;
};
