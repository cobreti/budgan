import 'reflect-metadata';
import {Container} from 'inversify';
import {type ILocalFilePicker, LocalFilePicker} from '@/services/localFilePicker';
import {ServicesTypes} from '@/services/types';

const container = new Container();

container.bind(ServicesTypes.LocalFilePicker).to(LocalFilePicker);

export {
    container
}
