
import { describe, test, expect, beforeEach, afterEach, vi } from 'vitest';
import {XmlParserSource} from '@XmlParser/XmlParserSource';
import {XmlParser} from '@XmlParser/XmlParser';


describe('XmlParser', async () => {

    var source: XmlParserSource;
    var parser: XmlParser;

    beforeEach( async() => {
        source = new XmlParserSource("<xml>some value</xml>");
        parser = new XmlParser(source);
    });

    afterEach( async() => {
        vi.resetAllMocks();
    })

    test('construction', () => {

        expect(parser.source_).toEqual(source);
    })

    test('handleChar with <', async () => {

        const onTagContentSpy = vi.spyOn(parser, 'onTagContent');

        parser.charAggr_ = "value".split('');
        parser.handleChar('<');

        expect(onTagContentSpy).toHaveBeenCalledWith('value');
        expect(parser.charAggr_).toEqual([]);
        expect(parser.handleFctPtr_).toEqual(parser.handleChar_InTag);
    });

    test('handleChar with character', async() => {

        parser.handleChar('t');

        expect(parser.charAggr_).toHaveLength(1);
        expect(parser.charAggr_[0]).toEqual('t');
    });


    test('handleChar_InTag with character', async() => {

        parser.handleChar_InTag('t');

        expect(parser.charAggr_).toHaveLength(1);
        expect(parser.charAggr_[0]).toEqual('t');
    });

    test('handleChar_InTag with >', async() => {

        const onOpeningTagSpy = vi.spyOn(parser, 'onOpeningTag');

        parser.charAggr_ = "tagValue".split('');
        parser.handleChar_InTag('>');

        expect(onOpeningTagSpy).toHaveBeenCalledWith('tagValue');
        expect(parser.charAggr_).toEqual([]);
        expect(parser.handleFctPtr_).toEqual(parser.handleChar);
    });

    test('handleChar_InTag with /', async() => {

        parser.handleChar_InTag('/');

        expect(parser.charAggr_).toEqual([]);
        expect(parser.handleFctPtr_).toEqual(parser.handleChar_InClosingTag);
    });

    test('handleChar_InClosingTag with a character value', async() => {

        parser.handleChar_InClosingTag('t');

        expect(parser.charAggr_).toHaveLength(1);
        expect(parser.charAggr_[0]).toEqual('t');
    });

    test('handleChar_InClosingTag with >', async() => {

        const onClosingTagSpy = vi.spyOn(parser, 'onClosingTag');

        parser.charAggr_ = 'tagValue'.split('');
        parser.handleChar_InClosingTag('>');

        expect(onClosingTagSpy).toHaveBeenCalledWith('tagValue');
        expect(parser.charAggr_).toEqual([]);
        expect(parser.handleFctPtr_).toEqual(parser.handleChar);
    });
})

