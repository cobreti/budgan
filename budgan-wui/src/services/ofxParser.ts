import {injectable} from 'inversify';

export interface IOfxParser {
    parse(ofxData: string) : void;
}

@injectable()
export class OfxParser {

    #tags = new Set<string>();

    #closingTags = new Set<string>();

    #charAggr :string[] = [];

    #handleFctPtr : (c: string) => void = this.handleChar;

    constructor() {
        console.debug('OfxParser constructor');
    }

    parse(ofxData: string) : void
    {
        const chars = [...ofxData];

        chars.forEach((c, i) => {
            this.#handleFctPtr.apply(this, [c]);
        })
    }

    handleChar(c : string) {

        switch (c) {
            case '<': {
                this.#charAggr = [];
                this.#handleFctPtr = this.handleInOpeningTagChar;
            }
            break;

            default: {
                this.#charAggr.push(c);
            }
        }


    }

    handleInOpeningTagChar(c : string) {

        switch (c) {
            case '>': {
                const tag = this.#charAggr.join("");
                if (!this.#tags.has(tag)) {
                    this.#tags.add(tag);
                    console.debug(`new tag found : ${tag}`);
                }

                this.#charAggr = [];
                this.#handleFctPtr = this.handleChar;
            }
            break;

            case '/': {
                this.#charAggr = [];
                this.#handleFctPtr = this.handleInClosingTagChar;
            }
            break;

            default: {
                this.#charAggr.push(c);
            }
        }
    }

    handleInClosingTagChar(c: string) {
        switch (c) {
            case '>': {
                const tag = this.#charAggr.join("");
                if (!this.#closingTags.has(tag)) {
                    this.#closingTags.add(tag);
                    console.debug(`new closing tag found : ${tag}`);
                }

                this.#charAggr = [];
                this.#handleFctPtr = this.handleChar;
            }
            break;

            default: {
                this.#charAggr.push(c);
            }
        }
    }
}
