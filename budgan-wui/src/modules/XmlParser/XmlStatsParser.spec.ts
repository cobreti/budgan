import { describe, beforeEach, expect, test } from 'vitest';
import {XmlParserSource} from '@XmlParser/XmlParserSource';
import {XmlStatsParser} from '@XmlParser/XmlStatsParser';

describe('XmlStatsParser', () => {

    const xmlValue = "<tag>value</tag>";
    var xmlSource : XmlParserSource;
    var xmlStatsParser : XmlStatsParser;

    beforeEach(() => {

        xmlSource = new XmlParserSource(xmlValue);
        xmlStatsParser = new XmlStatsParser(xmlSource);
    });

    test('first time encounter opening tag', () => {

            xmlStatsParser.onOpeningTag('tag');

            expect(xmlStatsParser.tagsStats_['tag'].openingCount).toBe(1);
            expect(xmlStatsParser.tagsStats_['tag'].closingCount).toBe(0);
    });

    test('second time encounter opening tag', () => {

        xmlStatsParser.tagsStats_['tag'] = {
            openingCount: 1,
            closingCount: 0
        };

        xmlStatsParser.onOpeningTag('tag');

        expect(xmlStatsParser.tagsStats_['tag'].openingCount).toBe(2);
        expect(xmlStatsParser.tagsStats_['tag'].closingCount).toBe(0);
    });

    test('first time encounter closing tag', () => {

        xmlStatsParser.onClosingTag('tag');

        expect(xmlStatsParser.tagsStats_['tag'].openingCount).toBe(0);
        expect(xmlStatsParser.tagsStats_['tag'].closingCount).toBe(1);
    });

    test('second time encounter closing tag', () => {

        xmlStatsParser.tagsStats_['tag'] = {
            openingCount: 0,
            closingCount: 1
        };

        xmlStatsParser.onClosingTag('tag');

        expect(xmlStatsParser.tagsStats_['tag'].openingCount).toBe(0);
        expect(xmlStatsParser.tagsStats_['tag'].closingCount).toBe(2);
    });

    test('postprocessing normal tag', () => {

        xmlStatsParser.tagsStats_['tag'] = {
            openingCount: 1,
            closingCount: 1
        };

        xmlStatsParser.onPostProcessing();

        expect(xmlStatsParser.normalTags_.has('tag')).toBe(true);
    });

    test('postprocessing non closing tag', () => {

        xmlStatsParser.tagsStats_['tag'] = {
            openingCount: 2,
            closingCount: 0
        };

        xmlStatsParser.onPostProcessing();

        expect(xmlStatsParser.nonClosingTags_.has('tag')).toBe(true);
    });

    test('postprocessing problematic closing tag', () => {

        xmlStatsParser.tagsStats_['tag'] = {
            openingCount: 2,
            closingCount: 1
        };

        xmlStatsParser.onPostProcessing();

        expect(xmlStatsParser.problematicTags_.has('tag')).toBe(true);
    });

    test('postprocessing non opening tag', () => {

        xmlStatsParser.tagsStats_['tag'] = {
            openingCount: 0,
            closingCount: 2
        };

        xmlStatsParser.onPostProcessing();

        expect(xmlStatsParser.nonOpeningTags_.has('tag')).toBe(true);
    });

    test('postprocessing problematic opening tag', () => {

        xmlStatsParser.tagsStats_['tag'] = {
            openingCount: 1,
            closingCount: 2
        };

        xmlStatsParser.onPostProcessing();

        expect(xmlStatsParser.problematicTags_.has('tag')).toBe(true);
    });

    test('nonClosingTag getter', () => {

        xmlStatsParser.nonClosingTags_.add('tag');

        expect(xmlStatsParser.nonClosingTags.has('tag')).toBe(true);
    });

    test('nonOpeningTag getter', () => {

        xmlStatsParser.nonOpeningTags_.add('tag');

        expect(xmlStatsParser.nonOpeningTags.has('tag')).toBe(true);
    });

    test('normalTag getter', () => {

        xmlStatsParser.normalTags_.add('tag');

        expect(xmlStatsParser.normalTags.has('tag')).toBe(true);
    });

    test('problematicTag getter', () => {

        xmlStatsParser.problematicTags_.add('tag');

        expect(xmlStatsParser.problematicTags.has('tag')).toBe(true);
    });
});

