export class XmlParserSource {

    xmlData_ : string[];

    constructor(data: string) {
        this.xmlData_ = [...data];
    }

    get xmlData(): string[] { return this.xmlData_; }
}
