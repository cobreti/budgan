import { Injectable, InjectionToken } from '@angular/core';
import { Result } from '@app-types/result';

export type CsvJsonRecord = Record<string, string>

export type CsvContentExtractionResult = {
  delimiter: string
  headerRowIndex: number
  header: string[]
  rows: CsvJsonRecord[]
}

type ParsedLine = {
  lineNumber: number
  fields: string[]
}

export interface CsvContentExtractorService {
  extract(csvText: string): Result<CsvContentExtractionResult>
}

export const CSV_CONTENT_EXTRACTOR_SERVICE = new InjectionToken<CsvContentExtractorService>('CsvContentExtractorService')

@Injectable({ providedIn: 'root' })
export class CsvContentExtractorServiceImpl implements CsvContentExtractorService {
  private readonly candidateDelimiters = [',', ';', '\t', '|']

  extract(csvText: string): Result<CsvContentExtractionResult> {
    try {
      const lines = csvText.split(/\r?\n/)
      const delimiter = this.detectDelimiter(lines)
      const parsedLines = this.parseLines(lines, delimiter)

      if (parsedLines.length === 0) {
        throw new Error('CSV content is empty')
      }

      const expectedColumnsCount = this.getMostCommonColumnsCount(parsedLines)
      const headerLine = this.detectHeaderLine(parsedLines, expectedColumnsCount)

      if (!headerLine) {
        throw new Error('Unable to detect a CSV header row')
      }

      const header = this.buildHeader(headerLine.fields)
      const rows = this.toJsonRows(parsedLines, headerLine.lineNumber, header, expectedColumnsCount)

      return {
        success: true,
        value: { delimiter, headerRowIndex: headerLine.lineNumber, header, rows }
      }
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to process CSV content'
      return { success: false, error: message }
    }
  }

  private detectDelimiter(lines: string[]): string {
    let bestDelimiter = this.candidateDelimiters[0]
    let bestScore = Number.NEGATIVE_INFINITY

    for (const delimiter of this.candidateDelimiters) {
      const parsedLines = this.parseLines(lines, delimiter)
      if (parsedLines.length === 0) {
        continue
      }

      const mostCommonColumnsCount = this.getMostCommonColumnsCount(parsedLines)
      const compatibleLinesCount = parsedLines.filter(
        (line) => line.fields.length === mostCommonColumnsCount && mostCommonColumnsCount > 1
      ).length

      const score = compatibleLinesCount * 10 + mostCommonColumnsCount
      if (score > bestScore) {
        bestScore = score
        bestDelimiter = delimiter
      }
    }

    return bestDelimiter
  }

  private parseLines(lines: string[], delimiter: string): ParsedLine[] {
    const parsedLines: ParsedLine[] = []

    for (let idx = 0; idx < lines.length; idx++) {
      const line = lines[idx]
      if (line.trim().length === 0) {
        continue
      }

      const fields = this.splitCsvLine(line, delimiter)
      parsedLines.push({ lineNumber: idx, fields })
    }

    return parsedLines
  }

  private splitCsvLine(line: string, delimiter: string): string[] {
    const fields: string[] = []
    let currentField = ''
    let inQuotes = false

    for (let idx = 0; idx < line.length; idx++) {
      const char = line[idx]
      const nextChar = idx + 1 < line.length ? line[idx + 1] : ''

      if (char === '"') {
        if (inQuotes && nextChar === '"') {
          currentField += '"'
          idx++
          continue
        }
        inQuotes = !inQuotes
        continue
      }

      if (char === delimiter && !inQuotes) {
        fields.push(currentField.trim())
        currentField = ''
        continue
      }

      currentField += char
    }

    fields.push(currentField.trim())
    return fields
  }

  private getMostCommonColumnsCount(parsedLines: ParsedLine[]): number {
    const counts = new Map<number, number>()

    for (const line of parsedLines) {
      const columnsCount = line.fields.length
      counts.set(columnsCount, (counts.get(columnsCount) ?? 0) + 1)
    }

    let bestColumnsCount = 0
    let bestFrequency = 0

    for (const [columnsCount, frequency] of counts) {
      if (frequency > bestFrequency) {
        bestColumnsCount = columnsCount
        bestFrequency = frequency
      }
    }

    return bestColumnsCount
  }

  private detectHeaderLine(parsedLines: ParsedLine[], expectedColumnsCount: number): ParsedLine | undefined {
    const candidates = parsedLines.filter((line) => line.fields.length === expectedColumnsCount)

    let bestCandidate: ParsedLine | undefined = undefined
    let bestScore = Number.NEGATIVE_INFINITY

    for (const candidate of candidates.slice(0, 10)) {
      const score = this.getHeaderScore(candidate.fields)
      if (score > bestScore) {
        bestScore = score
        bestCandidate = candidate
      }
    }

    return bestCandidate
  }

  private getHeaderScore(fields: string[]): number {
    let score = 0
    const normalizedFields = fields.map((field) => field.trim().toLowerCase())

    for (const field of normalizedFields) {
      if (field.length === 0) {
        score -= 2
        continue
      }
      if (/[a-zA-Z]/.test(field)) {
        score += 2
      }
      if (!this.looksLikeNumericValue(field)) {
        score += 1
      }
    }

    const uniqueCount = new Set(normalizedFields).size
    if (uniqueCount === normalizedFields.length) {
      score += 2
    }

    return score
  }

  private looksLikeNumericValue(value: string): boolean {
    if (value.length === 0) {
      return false
    }
    const numericValue = Number(value.replace(',', '.'))
    return !Number.isNaN(numericValue)
  }

  private buildHeader(rawHeader: string[]): string[] {
    const usedKeys = new Set<string>()

    return rawHeader.map((name, idx) => {
      const baseKey = this.normalizeHeader(name, idx)

      if (!usedKeys.has(baseKey)) {
        usedKeys.add(baseKey)
        return baseKey
      }

      let duplicateIndex = 2
      let keyCandidate = `${baseKey}_${duplicateIndex}`
      while (usedKeys.has(keyCandidate)) {
        duplicateIndex++
        keyCandidate = `${baseKey}_${duplicateIndex}`
      }

      usedKeys.add(keyCandidate)
      return keyCandidate
    })
  }

  private normalizeHeader(name: string, idx: number): string {
    const normalized = name
      .trim()
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, '_')
      .replace(/^_+|_+$/g, '')

    if (normalized.length === 0) {
      return `column_${idx + 1}`
    }

    return normalized
  }

  private toJsonRows(
    parsedLines: ParsedLine[],
    headerLineNumber: number,
    header: string[],
    expectedColumnsCount: number
  ): CsvJsonRecord[] {
    const rows = parsedLines.filter(
      (line) => line.lineNumber > headerLineNumber && line.fields.length === expectedColumnsCount
    )

    return rows.map((line) => {
      const jsonLine: CsvJsonRecord = {}
      for (let idx = 0; idx < header.length; idx++) {
        jsonLine[header[idx]] = line.fields[idx] ?? ''
      }
      return jsonLine
    })
  }
}
