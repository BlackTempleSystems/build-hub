# Authentication roadmap

## Current decision (v1)

Use **token-based auth** (access + refresh tokens).

Why:
- simplest path for early-stage self-hosting
- no external identity provider required
- enables UI + API development without committing to an OAuth provider too early

## Future options

- Add **OAuth** flows (e.g., Google/Microsoft) when needed
- Consider a **separate auth service** (or gateway) as the system grows

Migration plan idea:
- keep internal token format stable for clients
- add OAuth providers behind the API (exchange external identity → internal tokens)
- later split auth into a dedicated service if required
