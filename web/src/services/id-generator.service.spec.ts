import { TestBed } from '@angular/core/testing';
import { ID_GENERATOR_SERVICE, IdGeneratorServiceImpl } from './id-generator.service';

const UUID_V4_REGEX = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

describe('IdGeneratorServiceImpl', () => {
  let service: IdGeneratorServiceImpl;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IdGeneratorServiceImpl);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return a valid UUID v4', () => {
    expect(service.generateId()).toMatch(UUID_V4_REGEX);
  });

  it('should return a unique ID on each call', () => {
    const ids = Array.from({ length: 10 }, () => service.generateId());
    const unique = new Set(ids);
    expect(unique.size).toBe(10);
  });
});

describe('ID_GENERATOR_SERVICE token', () => {
  it('should resolve to an IdGeneratorServiceImpl instance', () => {
    TestBed.configureTestingModule({
      providers: [
        { provide: ID_GENERATOR_SERVICE, useClass: IdGeneratorServiceImpl },
      ],
    });
    const service = TestBed.inject(ID_GENERATOR_SERVICE);
    expect(service).toBeInstanceOf(IdGeneratorServiceImpl);
  });

  it('should generate valid UUIDs when injected via token', () => {
    TestBed.configureTestingModule({
      providers: [
        { provide: ID_GENERATOR_SERVICE, useClass: IdGeneratorServiceImpl },
      ],
    });
    const service = TestBed.inject(ID_GENERATOR_SERVICE);
    expect(service.generateId()).toMatch(UUID_V4_REGEX);
  });
});
