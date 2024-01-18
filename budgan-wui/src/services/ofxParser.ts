import {injectable} from 'inversify';

export interface IOfxParser {
    parse(ofxData: string) : void;
}

interface IXmlParser {

}

type TagsStatsEntry = {
    openingCount: number
    closingCount: number
};

type TagsStats = { [key:string]: TagsStatsEntry };


class XmlParser implements IXmlParser {

    charAggr_ :string[] = [];

    parseTime_: number = 0;
    totalChars_: number = 0;

    handleFctPtr_ : (c: string) => void = this.handleChar;

    get parseTime(): number { return this.parseTime_; }

    get totalChars(): number { return this.totalChars_; }

    parse(xmlData: string): void {

        const start = performance.now();

        const chars = [...xmlData];

        this.totalChars_ = chars.length;

        chars.forEach((c) => {
            this.handleFctPtr_.apply(this, [c]);
        });

        const end = performance.now();

        this.onPostProcessing();

        this.parseTime_ = end - start;
    }

    onPostProcessing(): void {}
    onOpeningTag(tagValue: string): void {}
    onClosingTag(tagValue: string): void {}
    onTagContent(tagContent: string): void {}


    handleChar(c : string) {

        switch (c) {
            case '<': {
                const value = this.charAggr_.join("");
                this.onTagContent(value);

                this.charAggr_ = [];
                this.handleFctPtr_ = this.handleChar_InTag;
            }
                break;

            default: {
                this.charAggr_.push(c);
            }
        }
    }

    handleChar_InTag(c : string) {

        switch (c) {
            case '>': {
                const tag = this.charAggr_.join("");

                this.onOpeningTag(tag);

                this.charAggr_ = [];
                this.handleFctPtr_ = this.handleChar;
            }
                break;

            case '/': {
                this.charAggr_ = [];
                this.handleFctPtr_ = this.handleChar_InClosingTag;
            }
                break;

            default: {
                this.charAggr_.push(c);
            }
        }
    }

    handleChar_InClosingTag(c: string) {
        switch (c) {
            case '>': {
                const tag = this.charAggr_.join("");

                this.onClosingTag(tag);

                this.charAggr_ = [];
                this.handleFctPtr_ = this.handleChar;
            }
                break;

            default: {
                this.charAggr_.push(c);
            }
        }
    }
}


class XmlStatsParser extends XmlParser {

    tagsStats_:TagsStats = {};

    nonClosingTags_ = new Set<string>();
    nonOpeningTags_ = new Set<string>();
    normalTags_ = new Set<string>();
    problematicTags_ = new Set<string>();

    get nonClosingTags(): Set<string> { return this.nonClosingTags_ };
    get nonOpeningTags(): Set<string> { return this.nonOpeningTags_ };
    get normalTags(): Set<string> { return this.normalTags_ };
    get problematicTags(): Set<string> { return this.problematicTags_ };

    onOpeningTag(tagValue: string): void {
        if (tagValue in this.tagsStats_) {
            this.tagsStats_[tagValue].openingCount ++;
        }
        else {
            this.tagsStats_[tagValue] = {
                openingCount: 1,
                closingCount: 0
            };
        }
    }

    onClosingTag(tagValue: string): void {
        if (tagValue in this.tagsStats_) {
            this.tagsStats_[tagValue].closingCount ++;
        }
        else {
            this.tagsStats_[tagValue] = {
                openingCount: 0,
                closingCount: 1
            };
        }

    }

    onTagContent(tagContent: string): void { }

    onPostProcessing() {
        for (var k in this.tagsStats_)
        {
            const stats = this.tagsStats_[k];
            const diff = stats.openingCount - stats.closingCount;

            if (diff == 0) {
                this.normalTags_.add(k);
            }

            if (diff > 0) {
                if (stats.closingCount == 0) {
                    this.nonClosingTags_.add(k);
                }
                else {
                    this.problematicTags_.add(k);
                }
            }

            if (diff < 0) {
                if (stats.openingCount == 0) {
                    this.nonOpeningTags_.add(k);
                }
                else {
                    this.problematicTags_.add(k);
                }
            }
        }
    }
}


@injectable()
export class OfxParser implements IOfxParser {

    constructor() {
        console.debug('OfxParser constructor');
    }

    parse(ofxData: string) : void
    {
        var xmlStatsParser = new XmlStatsParser();
        xmlStatsParser.parse(ofxData);

        console.debug('non-closing tags :', xmlStatsParser.nonClosingTags);
        console.debug('normal tags : ', xmlStatsParser.normalTags);
        console.debug('non-opening tags : ', xmlStatsParser.nonOpeningTags);
        console.debug('problematic tags : ', xmlStatsParser.problematicTags);
        console.debug('parsing time (ms) : ', xmlStatsParser.parseTime);
        console.debug('size (in characters) : ', xmlStatsParser.totalChars);
    }
}
