import 'reflect-metadata';
import {Container} from 'inversify';
import {ServicesTypes} from '@/services/types';
import {OfxParser} from '@/services/ofxParser';

const container = new Container();

container.bind(ServicesTypes.OfxParser).to(OfxParser);

export {
    container
}
