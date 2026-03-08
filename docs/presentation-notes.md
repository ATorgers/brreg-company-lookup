# Presentation Notes — brreg-company-lookup

## Start with the problem
- Norwegian company lookup requires hitting the Brønnøysundregisteret public API
- Needed a clean, maintainable solution — not just a quick script

## Walk through the layered architecture diagram
- Domain at the bottom — pure business concepts, no dependencies
- Application layer — defines what the system does, not how
- Infrastructure — the "how": HTTP calls and caching
- Api — the entry point: routes HTTP requests into the system
- Frontend — React app that talks to the backend

## Walk through the request flow diagram
- Every lookup goes through a query object that Mediator routes to the right handler
- The handler validates the org number before ever touching the network
- Clean separation: the endpoint knows nothing about validation or HTTP calls to Brreg

## Highlight the decorator pattern (component diagram)
- `CachedCompanyService` wraps `BrregCompanyService` transparently
- The rest of the system only knows about `ICompanyService` — caching is invisible
- Easy to remove, replace or add more decorators without touching business logic

## Talk about error handling
- Typed errors instead of exceptions — `Result<Company, CompanyError>`
- Each failure scenario maps to the correct HTTP status: 404, 422, 503, 500
- Errors are never cached — retries always hit the real service

## Mention the supporting bits
- Serilog rolling file logs for observability
- Unit tests covering validation, cache hits/misses, and error propagation
- Input validation both client-side (React) and server-side (value object)
