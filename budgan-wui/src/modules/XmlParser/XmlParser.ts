import type {XmlParserSource} from '@XmlParser/XmlParserSource';

export interface IXmlParser {

}


export class XmlParser implements IXmlParser {

    charAggr_ :string[] = [];

    parseTime_: number = 0;
    totalChars_: number = 0;

    handleFctPtr_ : (c: string) => void = this.handleChar;

    get parseTime(): number { return this.parseTime_; }

    get totalChars(): number { return this.totalChars_; }

    parse(source: XmlParserSource): void {

        const start = performance.now();

        this.totalChars_ = source.xmlData.length;

        source.xmlData.forEach((c) => {
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


