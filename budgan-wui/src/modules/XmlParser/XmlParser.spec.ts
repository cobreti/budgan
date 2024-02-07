
import { describe, test, expect, vi } from 'vitest'
import {XmlParserSource} from '@XmlParser/XmlParserSource';
import {XmlParser} from '@XmlParser/XmlParser';

describe('XmlParser', () => {

    test('currentNode null with empty stack', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);

        expect(xmlParser.currentNode).toBeNull();
    })

    test('currentNode with non-empty stack', () => {

            const parserSource = new XmlParserSource("<a><b></b></a>");
            const xmlParser = new XmlParser(parserSource);
            const node = {
                tag: "a",
                children: []
            };

            xmlParser.nodeStack_.push(node);

            expect(xmlParser.currentNode).to.equal(node);
    });

    test('xmlTree getter', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);

        expect(xmlParser.xmlTree).to.equal(xmlParser.xmlTree_);
    });

    test('headerContent getter', () => {

            const parserSource = new XmlParserSource("");
            const xmlParser = new XmlParser(parserSource);

            xmlParser.headerContent_ = ['version="1.0" encoding="UTF-8"'];

            expect(xmlParser.headerContent).toEqual(['version="1.0" encoding="UTF-8"']);
    });

    test('xmlTree getter', () => {
            
            const parserSource = new XmlParserSource("<a><b></b></a>");
            const xmlParser = new XmlParser(parserSource);
    
            expect(xmlParser.xmlTree).to.equal(xmlParser.xmlTree_);
    });

    test('removeCurrentNodeIfNonClosingTag with no current node', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);

        const hasSpy = vi.spyOn(parserSource.nonClosingTags_, 'has');

        xmlParser.removeCurrentNodeIfNonClosingTag();

        expect(hasSpy).not.toBeCalled();
    });

    test('removeCurrentNodeIfNonClosingTag with closing tag node', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        xmlParser.nodeStack_.push(node);

        xmlParser.removeCurrentNodeIfNonClosingTag();

        expect(xmlParser.nodeStack_.length).to.equal(1);
    });

    test('removeCurrentNodeIfNonClosingTag with non-closing tag node', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        const popSpy = vi.spyOn(xmlParser.nodeStack_, 'pop');

        xmlParser.nodeStack_.push(node);
        parserSource.nonClosingTags_.add("a");

        xmlParser.removeCurrentNodeIfNonClosingTag();

        expect(xmlParser.nodeStack_.length).to.equal(0);
        expect(popSpy).toBeCalled();
    });

    test('setTreeRoot with existing root', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);
        const rootNode = {
            tag: "a",
            children: []
        };
        const node = {
            tag: "b",
            children: []
        };

        xmlParser.xmlTree_.root = rootNode;

        xmlParser.setTreeRoot(node);

        expect(xmlParser.xmlTree_.root).to.equal(rootNode);
    });

    test('setTreeRoot with non-closing tag', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        parserSource.nonClosingTags_.add("a");

        xmlParser.setTreeRoot(node);

        expect(xmlParser.xmlTree_.root).toBeUndefined();
    });

    test('setTreeRoot with no root and no non-closing tag', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        xmlParser.setTreeRoot(node);

        expect(xmlParser.xmlTree_.root).toEqual(node);
    });

    test('onOpeningTag with no current node', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);

        xmlParser.onOpeningTag("a");

        expect(xmlParser.nodeStack_.length).to.equal(1);
        expect(xmlParser.nodeStack_[0].tag).to.equal("a");
        expect(xmlParser.nodeStack_[0].children.length).to.equal(0);
        expect(xmlParser.xmlTree_.root).toEqual(xmlParser.nodeStack_[0]);
    });

    test('onOpeningTag with current node', () => {

            const parserSource = new XmlParserSource("<a><b></b></a>");
            const xmlParser = new XmlParser(parserSource);
            const node = {
                tag: "a",
                children: []
            };

            xmlParser.nodeStack_.push(node);

            xmlParser.onOpeningTag("b");

            expect(xmlParser.nodeStack_.length).to.equal(2);
            expect(xmlParser.nodeStack_[1].tag).to.equal("b");
            expect(xmlParser.nodeStack_[1].children.length).to.equal(0);
            expect(xmlParser.nodeStack_[0].children.length).to.equal(1);
            expect(xmlParser.nodeStack_[0].children[0]).toEqual(xmlParser.nodeStack_[1]);
    });

    test('onClosingTag', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        xmlParser.nodeStack_.push(node);

        xmlParser.onClosingTag("a");

        expect(xmlParser.nodeStack_.length).to.equal(0);
    });

    test('onClosingTag outside of tree root', () => {

        const parserSource = new XmlParserSource("");
        const xmlParser = new XmlParser(parserSource);

        expect(() => xmlParser.onClosingTag("a")).toThrowError('closing tag a outside of tree root');
    });

    test('onClosingTag mismatch with current tag', () => {

        const parserSource = new XmlParserSource("");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        xmlParser.nodeStack_.push(node);

        expect(() => xmlParser.onClosingTag("b")).toThrowError('Invalid closing tag b for tag a');
    });

    test('onClosingTag with non-closing tag', () => {

        const parserSource = new XmlParserSource("");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        xmlParser.nodeStack_.push(node);
        parserSource.nonClosingTags_.add("a");

        expect(() => xmlParser.onClosingTag("a")).toThrowError('Non-closing tag a cannot have a closing tag');
    });

    test('onTagContent', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        xmlParser.nodeStack_.push(node);

        xmlParser.onTagContent("content");

        expect(xmlParser.nodeStack_.length).to.equal(1);
        expect(xmlParser.nodeStack_[0].content).toEqual("content");
    });

    test('onTagContent with content having spaces at start and end', () => {

        const parserSource = new XmlParserSource("<a><b></b></a>");
        const xmlParser = new XmlParser(parserSource);
        const node = {
            tag: "a",
            children: []
        };

        xmlParser.nodeStack_.push(node);

        xmlParser.onTagContent(" content ");

        expect(xmlParser.nodeStack_.length).to.equal(1);
        expect(xmlParser.nodeStack_[0].content).toEqual("content");
    });

    test('simple xml tree', () => {

        const parserSource = new XmlParserSource("<a>value a<b>value b</b></a>");
        const xmlParser = new XmlParser(parserSource);

        xmlParser.parse();

        expect(xmlParser.xmlTree_.root).toEqual({
            tag: "a",
            content: 'value a',
            children: [
                {
                    tag: "b",
                    content: 'value b',
                    children: []
                }
            ]
        });
    });

    test('xml tree with non-closing tag', () => {

        const parserSource = new XmlParserSource("<a>value a<b>value b<c>value c</c></a>");
        const xmlParser = new XmlParser(parserSource);

        parserSource.nonClosingTags_.add("b");

        xmlParser.parse();

        expect(xmlParser.xmlTree_.root).toEqual({
            tag: "a",
            content: 'value a',
            children: [
                {
                    tag: "b",
                    content: 'value b',
                    children: []
                },
                {
                    tag: "c",
                    content: 'value c',
                    children: []
                }
            ]
        });
    });

    test('xml tree with non-closing tag and no content', () => {

        const parserSource = new XmlParserSource("<a>value a<b><c>value c</c></a>");
        const xmlParser = new XmlParser(parserSource);

        parserSource.nonClosingTags_.add("b");

        xmlParser.parse();

        expect(xmlParser.xmlTree_.root).toEqual({
            tag: "a",
            content: 'value a',
            children: [
                {
                    tag: "b",
                    children: []
                },
                {
                    tag: "c",
                    content: 'value c',
                    children: []
                }
            ]
        });
    });

    test('xml tree with mutliple non-closing tags', () => {

        const parserSource = new XmlParserSource("<a>value a<b>value b<c>value c</c><d>value d</a>");
        const xmlParser = new XmlParser(parserSource);

        parserSource.nonClosingTags_.add("b");
        parserSource.nonClosingTags_.add("d");

        xmlParser.parse();

        expect(xmlParser.xmlTree_.root).toEqual({
            tag: "a",
            content: 'value a',
            children: [
                {
                    tag: "b",
                    content: 'value b',
                    children: []
                },
                {
                    tag: "c",
                    content: 'value c',
                    children: []
                },
                {
                    tag: "d",
                    content: 'value d',
                    children: []
                }
            ]
        });
    });

    test('xml tree with invalid tag being marked as non-closing but has a closing tag', () => {

        const parserSource = new XmlParserSource("<a>value a<b>value b</b><c>value c</c></a>");
        const xmlParser = new XmlParser(parserSource);

        parserSource.nonClosingTags_.add("b");

        expect(() => xmlParser.parse()).toThrowError('Non-closing tag b cannot have a closing tag');
    });

    test('xml tree with xml header values before opening tag', () => {

        const parserSource = new XmlParserSource("<?xml version=\"1.0\" encoding=\"UTF-8\"?><a>value a<b>value b</b></a>");
        const xmlParser = new XmlParser(parserSource);

        xmlParser.parse();

        expect(xmlParser.xmlTree_.root).toEqual({
            tag: "a",
            content: 'value a',
            children: [
                {
                    tag: "b",
                    content: 'value b',
                    children: []
                }
            ]
        });

        expect(xmlParser.headerContent_).toEqual(['version="1.0" encoding="UTF-8"']);
    });

    test('xml tree with non-xml header before opening tag', () => {

            const parserSource = new XmlParserSource("some header value<a>value a<b>value b</b></a>");
            const xmlParser = new XmlParser(parserSource);

            xmlParser.parse();

            expect(xmlParser.headerContent_).toEqual(['some header value']);
    })
});
