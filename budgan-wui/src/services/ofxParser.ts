import {injectable} from 'inversify';
import {XmlStatsParser} from '@XmlParser/XmlStatsParser';
import {XmlParserSource} from '@XmlParser/XmlParserSource';
import type {XmlNode} from '@XmlParser/XmlParser';
import {XmlParser} from '@XmlParser/XmlParser';
import type {OfxDocument} from '@models/ofxDocument';

export interface IOfxParser {
    parse(ofxData: string) : void;
}

type NodeHandlerContext = {
    ofxDoc: OfxDocument;
}

type NodeHandler = (context: NodeHandlerContext, node: XmlNode) => void;



@injectable()
export class OfxParser implements IOfxParser {

    ofxDoc_ : OfxDocument | null = null;

    tagHandlerTable_ : { [key: string]: NodeHandler } = {
        'DTSTART': this.handle_DTSTART,
        'DTEND': this.handle_DTEND
    }

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
        const xmlParser = new XmlParser(xmlSource);
        xmlParser.parse();

        console.debug('parsing time (ms) : ', xmlParser.parseTime);
        console.debug('size (in characters) : ', xmlParser.totalChars);
        console.debug('xml tree : ', xmlParser.xmlTree);
        console.debug('ofx header : ', xmlParser.headerContent);

        this.ofxDoc_ = this.createOfxDocument(xmlParser.headerContent);

        if (xmlParser.xmlTree.root) {
            let context : NodeHandlerContext = {
                ofxDoc: this.ofxDoc_
            };
            this.iterateOfxNodeContent(context, xmlParser.xmlTree.root);
        }

        console.log('ofxDocument : ', this.ofxDoc_);
    }

    iterateOfxNodeContent(context: NodeHandlerContext, node: XmlNode) {

        if (node.tag in this.tagHandlerTable_) {
            const handler = this.tagHandlerTable_[node.tag];
            handler.call(this, context, node);
        }

        if (node.children) {
            node.children.forEach((child) => {
                this.iterateOfxNodeContent(context, child);
            });
        }
    }

    createOfxDocument(header: string[]) : OfxDocument {

        let ofxVersion = '';
        let ofxSecurity = '';
        let ofxEncoding = '';
        let ofxCharset = '';
        let ofxCompression = '';

        const ofxVersionRegex = /VERSION:(\d+)/;
        const ofxSecurityRegex = /SECURITY:(\w+)/;
        const ofxEncodingRegex = /ENCODING:(\w+)/;
        const ofxCharsetRegex = /CHARSET:(\w+)/;
        const ofxCompressionRegex = /COMPRESSION:(\w+)/;

        header.forEach((line) => {
            ofxVersion = this.getRegexStringMatch(ofxVersionRegex, line);
            ofxSecurity = this.getRegexStringMatch(ofxSecurityRegex, line);
            ofxEncoding = this.getRegexStringMatch(ofxEncodingRegex, line);
            ofxCharset = this.getRegexStringMatch(ofxCharsetRegex, line);
            ofxCompression = this.getRegexStringMatch(ofxCompressionRegex, line);
        });

        return {
            version: ofxVersion,
            security: ofxSecurity,
            encoding: ofxEncoding,
            charset: ofxCharset,
            compression: ofxCompression
        };
    }

    getRegexStringMatch(regex: RegExp, line: string) : string {
        var match = regex.exec(line);
        if (match) {
            return match[1];
        }
        return '';
    }

    handle_DTSTART(context: NodeHandlerContext, node: XmlNode) {

        if (node.content) {
            context.ofxDoc.startDate = this.dateFromOfxDateValue(node.content);
        }
    }

    handle_DTEND(context: NodeHandlerContext, node: XmlNode) {

        if (node.content) {
            context.ofxDoc.endDate = this.dateFromOfxDateValue(node.content);
        }
    }

    dateFromOfxDateValue(ofxDate: string) {
        const regex = /(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<min>\d{2})(?<sec>\d{2})\.(?<msec>\d+)\[(?<tz_sign>-?\+?)(?<tz_value>\d+):(?<tz_name>\w*)\]/;

        const match = regex.exec(ofxDate);
        if (!match) {
            throw new Error(`Invalid ofx ate format : ${ofxDate}`);
        }

        const dateStr = `${match.groups?.year}-${match.groups?.month}-${match.groups?.day}T${match.groups?.hour}:${match.groups?.min}:${match.groups?.sec}.${match.groups?.msec}${match.groups?.tz_sign}${match.groups?.tz_value.padStart(2, '0')}:00`;
        return new Date(dateStr);
    }
}
