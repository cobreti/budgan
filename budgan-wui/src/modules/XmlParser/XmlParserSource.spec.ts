import { describe, beforeEach, test, expect } from 'vitest';
import {XmlParserSource} from '@XmlParser/XmlParserSource';

describe('XmlParserSource', () => {

    const xmlData = '<root>text</root>';
    var parserSource : XmlParserSource;

    beforeEach(() => {
        parserSource = new XmlParserSource(xmlData);
    })

    test('construction', () => {
        expect(parserSource.xmlData).toEqual(['<', 'r', 'o', 'o', 't', '>', 't', 'e', 'x', 't', '<', '/', 'r', 'o', 'o', 't', '>']);
        expect(parserSource.nonClosingTags.size).toEqual(0);
    });

    test('nonClosingTags setter/getter', () => {
        parserSource.nonClosingTags = new Set<string>(['root']);
        expect(parserSource.nonClosingTags.size).toEqual(1);
        expect(parserSource.nonClosingTags.has('root')).toBeTruthy();
    });
});
