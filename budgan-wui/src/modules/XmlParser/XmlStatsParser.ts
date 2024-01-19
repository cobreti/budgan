import {XmlParser} from '@XmlParser/XmlParser';
import type {TagsStats} from '@XmlParser/XmlTypes';

export class XmlStatsParser extends XmlParser {

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
