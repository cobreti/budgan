
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

        xmlParser.nodeStack_.push(node);
        parserSource.nonClosingTags_.add("a");

        xmlParser.removeCurrentNodeIfNonClosingTag();

        expect(xmlParser.nodeStack_.length).to.equal(0);
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
});
