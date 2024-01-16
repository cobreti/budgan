import {injectable} from 'inversify';

export interface IOfxParser {

}

@injectable()
export class OfxParser {

    #tags = new Set<string>();

    constructor() {
        console.debug('OfxParser constructor');
    }

    parse(ofxData: string)
    {

    }
}
