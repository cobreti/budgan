
import { describe, test, expect, beforeEach, afterEach, vi } from 'vitest';
import {XmlParserSource} from '@XmlParser/XmlParserSource';
import {XmlSyntaxParser} from '@XmlParser/XmlSyntaxParser';


describe('XmlSyntaxParser', async () => {

    const xmlData = '<xml>some value</xml>';
    var source: XmlParserSource;
    var parser: XmlSyntaxParser;

    beforeEach( async() => {
        source = new XmlParserSource(xmlData);
        parser = new XmlSyntaxParser(source);
    });

    afterEach( async() => {
        vi.resetAllMocks();
    })

    test('construction', () => {

        expect(parser.source).toEqual(source);
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

    test('parse', async() => {

            var nowValue = 0;

            const onOpeningTagSpy = vi.spyOn(parser, 'onOpeningTag');
            const onClosingTagSpy = vi.spyOn(parser, 'onClosingTag');
            const onTagContentSpy = vi.spyOn(parser, 'onTagContent');
            const onPostProcessingSpy = vi.spyOn(parser, 'onPostProcessing');
            const perfNowSpy = vi.spyOn(performance, 'now').mockImplementation(() => {
                return nowValue ++;
            });

            parser.parse();

            expect(onOpeningTagSpy).toHaveBeenCalledWith('xml');
            expect(onClosingTagSpy).toHaveBeenCalledWith('xml');
            expect(onTagContentSpy).toHaveBeenCalledWith('some value');
            expect(onPostProcessingSpy).toHaveBeenCalled();
            expect(parser.totalChars).toEqual(xmlData.length);
            expect(parser.parseTime).toEqual(1);
    });
})

