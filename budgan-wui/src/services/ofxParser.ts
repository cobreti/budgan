import {injectable} from 'inversify';
import {XmlStatsParser} from '@XmlParser/XmlStatsParser';
import {XmlParserSource} from '@XmlParser/XmlParserSource';
import {XmlTreeParser} from '@XmlParser/XmlTreeParser';

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
        const xmlSource = new XmlParserSource(ofxData);
        const xmlStatsParser = new XmlStatsParser(xmlSource);
        xmlStatsParser.parse();

        console.debug('non-closing tags :', xmlStatsParser.nonClosingTags);
        console.debug('normal tags : ', xmlStatsParser.normalTags);
        console.debug('non-opening tags : ', xmlStatsParser.nonOpeningTags);
        console.debug('problematic tags : ', xmlStatsParser.problematicTags);
        console.debug('parsing time (ms) : ', xmlStatsParser.parseTime);
        console.debug('size (in characters) : ', xmlStatsParser.totalChars);

        xmlSource.nonClosingTags = xmlStatsParser.nonClosingTags;
        const xmlTreeParser = new XmlTreeParser(xmlSource);
        xmlTreeParser.parse();

        console.debug('parsing time (ms) : ', xmlTreeParser.parseTime);
        console.debug('size (in characters) : ', xmlTreeParser.totalChars);
        console.debug('xml tree : ', xmlTreeParser.xmlTree);
    }
}
