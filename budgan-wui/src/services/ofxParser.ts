import {injectable} from 'inversify';
import {XmlStatsParser} from '@XmlParser/XmlStatsParser';
import {XmlParserSource} from '@XmlParser/XmlParserSource';
import type {XmlNode} from '@XmlParser/XmlParser';
import {XmlParser} from '@XmlParser/XmlParser';
import type {OfxDocument, OfxTransaction} from '@models/ofxDocument';

export interface IOfxParser {
    parse(ofxData: string) : OfxParseResult;
}

export type OfxParseResult = {

    document?: OfxDocument;
    header?: string[];
    parsingTime?: number;
}

type NodeHandlerContext = {
    ofxDoc: OfxDocument;
    ofxTransaction?: OfxTransaction;
}

type NodeHandlerFct = (context: NodeHandlerContext, node: XmlNode) => void;

type NodeHandlers = {
    openHandler?: NodeHandlerFct
    closeHandler?: NodeHandlerFct
}

@injectable()
export class OfxParser implements IOfxParser {

    ofxDoc_ : OfxDocument | undefined = undefined;

    tagHandlerTable_ : { [key: string]: NodeHandlers } = {
        'DTSTART': {
            openHandler: this.handle_open_DTSTART
        },
        'DTEND': {
            openHandler: this.handle_open_DTEND
        },
        'CURDEF': {
            openHandler: this.handle_open_CURDEF
        },
        'ACCTTYPE': {
            openHandler: this.handle_open_ACCTTYPE
        },
        'ACCTID': {
            openHandler: this.handle_open_ACCTID
        },
        'STMTTRN': {
            openHandler: this.handle_open_STMTTRN,
            closeHandler: this.handle_close_STMTTRN
        },
        'TRNTYPE': {
            openHandler: this.handle_open_TRNTYPE
        },
        'DTPOSTED': {
            openHandler: this.handle_open_DTPOSTED
        },
        'TRNAMT': {
            openHandler: this.handle_open_TRNAMT
        },
        'NAME': {
            openHandler: this.handle_open_NAME
        },
        'FITID': {
            openHandler: this.handle_open_FITID
        }
    }

    constructor() {
        console.debug('OfxParser constructor');
    }

    parse(ofxData: string) : OfxParseResult
    {
        const xmlSource = new XmlParserSource(ofxData);
        const xmlStatsParser = new XmlStatsParser(xmlSource);
        const xmlParser = new XmlParser(xmlSource);
        xmlStatsParser.parse();

        try {
            console.debug('non-closing tags :', xmlStatsParser.nonClosingTags);
            console.debug('normal tags : ', xmlStatsParser.normalTags);
            console.debug('non-opening tags : ', xmlStatsParser.nonOpeningTags);
            console.debug('problematic tags : ', xmlStatsParser.problematicTags);
            console.debug('parsing time (ms) : ', xmlStatsParser.parseTime);
            console.debug('size (in characters) : ', xmlStatsParser.totalChars);

            xmlSource.nonClosingTags = xmlStatsParser.nonClosingTags;

            xmlParser.parse();

            console.debug('parsing time (ms) : ', xmlParser.parseTime);
            console.debug('size (in characters) : ', xmlParser.totalChars);
            console.debug('xml tree : ', xmlParser.xmlTree);
            console.debug('ofx header : ', xmlParser.headerContent);

            this.ofxDoc_ = this.createOfxDocument(xmlParser.headerContent);

            if (xmlParser.xmlTree.root) {
                let context: NodeHandlerContext = {
                    ofxDoc: this.ofxDoc_
                };
                this.iterateOfxNodeContent(context, xmlParser.xmlTree.root);
            }
        }
        catch (e) {
            console.error('Error parsing ofx data : ', e);
        }

        return {
            document: this.ofxDoc_,
            header: xmlParser.headerContent,
            parsingTime: xmlStatsParser.parseTime + xmlParser.parseTime
        }
    }

    iterateOfxNodeContent(context: NodeHandlerContext, node: XmlNode) {

        if (node.tag in this.tagHandlerTable_) {
            const handler = this.tagHandlerTable_[node.tag].openHandler;
            handler?.call(this, context, node);
        }

        if (node.children) {
            node.children.forEach((child) => {
                this.iterateOfxNodeContent(context, child);
            });
        }

        if (node.tag in this.tagHandlerTable_) {
            const handler = this.tagHandlerTable_[node.tag].closeHandler;
            handler?.call(this, context, node);
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
            compression: ofxCompression,
            transactions: [],
        };
    }

    getRegexStringMatch(regex: RegExp, line: string) : string {
        var match = regex.exec(line);
        if (match) {
            return match[1];
        }
        return '';
    }

    handle_open_DTSTART(context: NodeHandlerContext, node: XmlNode) {

        if (node.content) {
            context.ofxDoc.startDate = this.dateFromOfxDateValue(node.content);
        }
    }

    handle_open_DTEND(context: NodeHandlerContext, node: XmlNode) {

        if (node.content) {
            context.ofxDoc.endDate = this.dateFromOfxDateValue(node.content);
        }
    }

    handle_open_CURDEF(context: NodeHandlerContext, node: XmlNode) {
        if (node.content) {
            context.ofxDoc.currency = node.content;
        }
    };

    handle_open_ACCTTYPE(context: NodeHandlerContext, node: XmlNode) {
        if (node.content) {
            context.ofxDoc.accountType = node.content;
        }
    }

    handle_open_ACCTID(context: NodeHandlerContext, node: XmlNode) {
        if (node.content) {
            context.ofxDoc.accountId = node.content;
        }
    }

    handle_open_STMTTRN(context: NodeHandlerContext, node: XmlNode) {
        if (context.ofxTransaction) {
            throw new Error("Nested transactions are not supported");
        }

        context.ofxTransaction = {

        };
    }

    handle_close_STMTTRN(context: NodeHandlerContext, node: XmlNode) {
        if (!context.ofxTransaction) {
            throw new Error("No transaction to close");
        }

        context.ofxDoc.transactions.push(context.ofxTransaction);
        context.ofxTransaction = undefined;
    }

    handle_open_TRNTYPE(context: NodeHandlerContext, node: XmlNode) {
        if (!context.ofxTransaction) {
            throw new Error("No transaction to set type");
        }

        if (node.content) {
            context.ofxTransaction.type = node.content;
        }
    }

    handle_open_DTPOSTED(context: NodeHandlerContext, node: XmlNode) {
        if (!context.ofxTransaction) {
            throw new Error("No transaction to set date");
        }

        if (node.content) {
            context.ofxTransaction.datePosted = this.dateFromOfxDateValue(node.content);
        }
    }

    handle_open_TRNAMT(context: NodeHandlerContext, node: XmlNode) {
        if (!context.ofxTransaction) {
            throw new Error("No transaction to set amount");
        }

        if (node.content) {
            context.ofxTransaction.amount = parseFloat(node.content);
        }
    }

    handle_open_NAME(context: NodeHandlerContext, node: XmlNode) {
        if (!context.ofxTransaction) {
            throw new Error("No transaction to set name");
        }

        if (node.content) {
            context.ofxTransaction.name = node.content;
        }
    }

    handle_open_FITID(context: NodeHandlerContext, node: XmlNode) {
        if (!context.ofxTransaction) {
            throw new Error("No transaction to set fitId");
        }

        if (node.content) {
            context.ofxTransaction.fitId = node.content;
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
