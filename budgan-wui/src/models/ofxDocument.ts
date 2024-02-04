export type OfxDocument = {
    version: string;
    security: string;
    encoding: string;
    charset: string;
    compression: string;

    startDate?: Date;
    endDate?: Date;
};
