import {injectable} from 'inversify';
import {XmlStatsParser} from '@XmlParser/XmlStatsParser';

export interface IOfxParser {
    parse(ofxData: string) : void;
}






@injectable()
export class OfxParser implements IOfxParser {

    constructor() {
        console.debug('OfxParser constructor');
    }

    parse(ofxData: string) : void
    {
        var xmlStatsParser = new XmlStatsParser();
        xmlStatsParser.parse(ofxData);

        console.debug('non-closing tags :', xmlStatsParser.nonClosingTags);
        console.debug('normal tags : ', xmlStatsParser.normalTags);
        console.debug('non-opening tags : ', xmlStatsParser.nonOpeningTags);
        console.debug('problematic tags : ', xmlStatsParser.problematicTags);
        console.debug('parsing time (ms) : ', xmlStatsParser.parseTime);
        console.debug('size (in characters) : ', xmlStatsParser.totalChars);
    }
}
