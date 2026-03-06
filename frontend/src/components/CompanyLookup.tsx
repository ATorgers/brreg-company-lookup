import { useState, useCallback } from 'react'

interface Company {
  organizationNumber: string
  organizationName: string
  companyType: string
  languageForm: string
}

const FIELD_LABELS: Record<keyof Company, string> = {
  organizationNumber: 'Organization Number',
  organizationName: 'Organization Name',
  companyType: 'Company Type',
  languageForm: 'Language Form',
}

const FIELD_ORDER: (keyof Company)[] = [
  'organizationNumber',
  'organizationName',
  'companyType',
  'languageForm',
]

function validateInput(value: string): string | null {
  if (!value.trim()) return 'Please enter an organization number.'
  if (!/^\d{9}$/.test(value)) return 'Organization number must be exactly 9 digits.'
  return null
}

function ReadOnlyField({ label, value }: { label: string; value: string }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '0.25rem' }}>
      <span style={{
        fontSize: '0.75rem',
        fontWeight: 600,
        textTransform: 'uppercase',
        letterSpacing: '0.06em',
        color: '#6b7280',
      }}>
        {label}
      </span>
      <div style={{
        padding: '0.5rem 0.75rem',
        background: '#f9fafb',
        border: '1px solid #e5e7eb',
        borderRadius: '6px',
        fontSize: '0.9375rem',
        color: '#111827',

      }}>
        {value || <span style={{ color: '#9ca3af', fontStyle: 'italic' }}>—</span>}
      </div>
    </div>
  )
}

export default function CompanyLookup() {
  const [input, setInput] = useState('')
  const [validationError, setValidationError] = useState<string | null>(null)
  const [company, setCompany] = useState<Company | null>(null)
  const [loading, setLoading] = useState(false)
  const [apiError, setApiError] = useState<string | null>(null)
  const [copied, setCopied] = useState(false)

  const handleLookup = useCallback(async () => {
    const error = validateInput(input)
    if (error) {
      setValidationError(error)
      return
    }

    setValidationError(null)
    setApiError(null)
    setCompany(null)
    setCopied(false)
    setLoading(true)

    try {
      const response = await fetch(`/api/companies/${input.trim()}`)

      if (!response.ok) {
        const problem = await response.json().catch(() => null)
        setApiError(problem?.detail ?? `Error ${response.status}: ${response.statusText}`)
        return
      }

      const data: Company = await response.json()
      setCompany(data)
    } catch {
      setApiError('Could not connect to the server. Is the API running?')
    } finally {
      setLoading(false)
    }
  }, [input])

  const handleCopy = useCallback(async () => {
    if (!company) return
    const text = FIELD_ORDER
      .map(key => `${FIELD_LABELS[key]}: ${company[key]}`)
      .join('\n')
    await navigator.clipboard.writeText(text)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }, [company])

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') handleLookup()
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const digits = e.target.value.replace(/\D/g, '')
    setInput(digits)
    setValidationError(null)
  }

  return (
    <div>
      {/* Input row */}
      <div style={{ display: 'flex', gap: '0.5rem' }}>
        <input
          type="text"
          inputMode="numeric"
          value={input}
          onChange={handleChange}
          onKeyDown={handleKeyDown}
          placeholder="e.g. 974760673"
          maxLength={9}
          aria-label="Organization number"
          style={{
            flex: 1,
            padding: '0.625rem 0.875rem',
            fontSize: '1rem',
            border: `1.5px solid ${validationError ? '#dc2626' : '#d1d5db'}`,
            borderRadius: '6px',
            outline: 'none',
            letterSpacing: '0.05em',
          }}
        />
        <button
          onClick={handleLookup}
          disabled={loading}
          style={{
            padding: '0.625rem 1.375rem',
            fontSize: '1rem',
            fontWeight: 600,
            background: loading ? '#93c5fd' : '#1d4ed8',
            color: '#fff',
            border: 'none',
            borderRadius: '6px',
            cursor: loading ? 'not-allowed' : 'pointer',
            whiteSpace: 'nowrap',
            transition: 'background 0.15s',
          }}
        >
          {loading ? 'Looking up…' : 'Look up'}
        </button>
      </div>

      {/* Validation error */}
      {validationError && (
        <p style={{ color: '#dc2626', fontSize: '0.875rem', marginTop: '0.375rem' }}>
          {validationError}
        </p>
      )}

      {/* API error */}
      {apiError && (
        <div style={{
          marginTop: '1rem',
          padding: '0.75rem 1rem',
          background: '#fef2f2',
          border: '1px solid #fca5a5',
          borderRadius: '6px',
          color: '#b91c1c',
          fontSize: '0.9rem',
        }}>
          {apiError}
        </div>
      )}

      {/* Result */}
      {company && (
        <div style={{ marginTop: '1.75rem' }}>
          {/* Result header with copy button */}
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '1rem' }}>
            <span style={{
              fontSize: '0.75rem',
              fontWeight: 600,
              textTransform: 'uppercase',
              letterSpacing: '0.06em',
              color: '#6b7280',
            }}>
              Company Details
            </span>
            <button
              onClick={handleCopy}
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '0.375rem',
                padding: '0.375rem 0.75rem',
                fontSize: '0.8125rem',
                fontWeight: 500,
                background: copied ? '#f0fdf4' : '#f9fafb',
                color: copied ? '#15803d' : '#374151',
                border: `1px solid ${copied ? '#86efac' : '#d1d5db'}`,
                borderRadius: '6px',
                cursor: 'pointer',
                transition: 'all 0.15s',
              }}
            >
              {copied ? '✓ Copied' : '⎘ Copy'}
            </button>
          </div>

          {/* Fields */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
            {FIELD_ORDER.map(key => (
              <ReadOnlyField
                key={key}
                label={FIELD_LABELS[key]}
                value={company[key]}
              />
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
