import {injectable} from 'inversify';
import {XmlStatsParser} from '@XmlParser/XmlStatsParser';
import {XmlParserSource} from '@XmlParser/XmlParserSource';

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
        const xmlStatsParser = new XmlStatsParser();
        const xmlSource = new XmlParserSource(ofxData);
        xmlStatsParser.parse(xmlSource);

        console.debug('non-closing tags :', xmlStatsParser.nonClosingTags);
        console.debug('normal tags : ', xmlStatsParser.normalTags);
        console.debug('non-opening tags : ', xmlStatsParser.nonOpeningTags);
        console.debug('problematic tags : ', xmlStatsParser.problematicTags);
        console.debug('parsing time (ms) : ', xmlStatsParser.parseTime);
        console.debug('size (in characters) : ', xmlStatsParser.totalChars);
    }
}
