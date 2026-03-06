import CompanyLookup from './components/CompanyLookup'

export default function App() {
  return (
    <main style={{
      fontFamily: 'system-ui, -apple-system, sans-serif',
      maxWidth: '640px',
      margin: '4rem auto',
      padding: '0 1.5rem',
      color: '#111',
    }}>
      <img
        src="/brreg-logo.png"
        alt="Brønnøysundregisteret"
        style={{ height: '2.5rem', marginBottom: '1.5rem', display: 'block' }}
      />
      <p style={{ color: '#555', marginBottom: '2rem' }}>
        Enter a 9-digit Norwegian organization number to retrieve company information
        from the Brønnøysund Register Centre.
      </p>
      <CompanyLookup />
    </main>
  )
}
