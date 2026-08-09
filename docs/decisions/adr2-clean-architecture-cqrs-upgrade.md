# ADR-NNN: Standardize CloudCanvas on Clean Architecture with Selective CQRS

## Status

Proposed

## Date

2026-07-03

## Context

At this poiunt, CloudCanvas has already moved meaningfully away from its earlier repository shape. On the current `dev` branch, the solution is organized into `CloudCanvas.Api`, `CloudCanvas.Application`, `CloudCanvas.Domain`, `CloudCanvas.Infrastructure`, and dedicated Azure Functions projects, which is much closer to a Clean Architecture layout than the earlier branch structure.

By comparison, the earlier `main` branch relied more heavily on a broad `CloudCanvas.Shared` package and did not show the same explicit project split around domain, application, and infrastructure boundaries. That older shape made it easier for technical contracts, shared utilities, and infrastructure-oriented concerns to accumulate in one place.

The current repository also already contains at least one explicit CQRS-style decision: the gallery-page ADR chooses SQL Server as the transactional source of truth and Cosmos DB as a read-model store for page rendering. At the same time, `dev` does not clearly have repository-wide CQRS implementation markers such as **MediatR** or pervasive command/query handler patterns.

## Decision

I will treat CloudCanvas as a system that is **being upgraded to Clean Architecture**, not as one that has already completed that journey. I will also **adopt CQRS selectively** where it creates a clear advantage, especially for read-heavy views, projections, orchestration-heavy workflows, and integration boundaries.

The intended architectural shape is:
- **Domain** for business concepts, invariants, and core rules.
- **Application** for use cases, commands, queries, handlers, validation, and abstraction boundaries.
- **Infrastructure** for SQL Server, Cosmos DB, Blob Storage, Service Bus, identity, and other external dependencies.
- **API and Azure Functions** as entry points that translate transport concerns into application requests.

**CQRS is not a blanket rule for every endpoint.** It should be used where reads and writes genuinely diverge in shape, scale, performance, or consistency expectations.

## Rationale

The current `dev` branch shows that the repository is already moving in the right direction structurally. The separate Application, Domain, Infrastructure, and API projects are a strong sign that the codebase is being reorganized around clearer boundaries than before.

However, structure alone is not enough to claim full Clean Architecture and CQRS adoption. To make that claim credibly, 
- the inner layers need to own use-case orchestration consistently, 
- outer layers need to remain thin, 
- CQRS needs to exist as an intentional application pattern rather than only as an isolated read-model decision.

This ADR therefore **documents the target state and the standard I want future changes to meet.** It gives a clear north star without overstating the maturity of the current implementation.

## Consequences

**Easier:**
- New work can be placed according to responsibility instead of convenience.
- The API and Azure Functions can converge on the same application-core patterns.
- Read models, projections, and async workflows gain a clearer architectural home.
- The repository becomes easier to review for dependency direction and boundary violations.

**Harder:**
- Existing logic will need to be pulled out of transport and infrastructure layers where those concerns have leaked inward.
- Some shared abstractions from the older repository shape will need to be split, reduced, or removed.
- CQRS will need discipline to avoid becoming ceremony in places where a simple transactional flow is enough.
