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

    get currentNode() : XmlNode | null {

        if (this.nodeStack_.length == 0) {
            return null;
        }

        return this.nodeStack_[this.nodeStack_.length-1];
    }

    get xmlTree(): XmlTree { return this.xmlTree_; }


    removeCurrentNodeIfNonClosingTag() {
        const node = this.currentNode;

        if (node === null) {
            return;
        }

        const isNonClosingTag = this.source_.nonClosingTags.has(node.tag);
        if (isNonClosingTag) {
            this.nodeStack_.pop();
        }
    }

    setTreeRoot(node: XmlNode) {
        if (this.xmlTree_.root) {
            return;
        }

        if (this.source_.nonClosingTags.has(node.tag)) {
            return;
        }

        this.xmlTree_.root = node;
    }

    onOpeningTag(tagValue: string): void {

        const node : XmlNode = {
            tag: tagValue,
            children: []
        };

        this.removeCurrentNodeIfNonClosingTag();

        const currentNode = this.currentNode;

        if (currentNode) {
            currentNode.children = [...currentNode.children, node];
        }
        else {
            this.setTreeRoot(node);
        }

        this.nodeStack_.push(node);
    }

    onClosingTag(tagValue: string): void {
        this.removeCurrentNodeIfNonClosingTag();

        this.nodeStack_.pop();
    }

    onTagContent(tagContent: string): void {
        const currentNode = this.currentNode;

        if (currentNode) {
            currentNode.content = tagContent.trim();
        }
    }

    onPostProcessing() {
    }

}
