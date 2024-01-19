export class XmlParserSource {

    xmlData_ : string[];

    nonClosingTags_ : Set<string> = new Set<string>();

    constructor(data: string) {
        this.xmlData_ = [...data];
    }

    get xmlData(): string[] { return this.xmlData_; }
    get nonClosingTags(): Set<string> { return this.nonClosingTags_; }
    set nonClosingTags(value:Set<string>) { this.nonClosingTags_ = value; }
}
