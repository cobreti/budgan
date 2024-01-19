import {XmlParser} from '@XmlParser/XmlParser';

export type XmlNode = {
    tag: string;
    content?: string;
    children: XmlNode[];
};


export type XmlTree = {
    root?: XmlNode;
}


export class XmlTreeParser extends XmlParser {

    xmlTree_ : XmlTree = {
    };

    nodeStack_: XmlNode[] = [];

    get currentNode() : XmlNode { return this.nodeStack_[this.nodeStack_.length-1]; }

    get xmlTree(): XmlTree { return this.xmlTree_; }

    onOpeningTag(tagValue: string): void {

        const node : XmlNode = {
            tag: tagValue,
            children: []
        };

        if (this.nodeStack_.length > 0) {
            const currentNode = this.currentNode;
            const isNonClosingTag = this.source_.nonClosingTags.has(currentNode.tag);

            if (isNonClosingTag) {
                this.nodeStack_.pop();
            }
        }

        if (this.nodeStack_.length > 0) {
            const currentNode = this.currentNode;

            currentNode.children = [...currentNode.children, node];
        }
        else {
            if (!this.xmlTree_.root && !this.source_.nonClosingTags.has(tagValue)) {
                this.xmlTree_.root = node;
            }
        }

        this.nodeStack_.push(node);
    }

    onClosingTag(tagValue: string): void {
        const currentNode = this.currentNode;
        const isNonClosingTag = this.source_.nonClosingTags.has(currentNode.tag);
        if (isNonClosingTag) {
            this.nodeStack_.pop();
        }

        this.nodeStack_.pop();
    }

    onTagContent(tagContent: string): void {
        if (this.nodeStack_.length > 0) {
            this.currentNode.content = tagContent.trim();
        }
    }

    onPostProcessing() {
    }

}
