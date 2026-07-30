# ADR: Continue the feature-based clean architecture and CQRS direction

## Status

Accepted

## Date

2026-07-30

## Context

CloudCanvas has already moved well beyond the earlier shared-all-the-things repository shape. The solution is now split into clearer boundaries with 



- **`CloudCanvas.Api`**

- **`CloudCanvas.Application`**
 
- **`CloudCanvas.Domain`**
 
- **`CloudCanvas.Infrastructure`**
 
- **`CloudCanvas.Functions.ThumbnailOrchestrator`**



and dedicated a Durable Functions projects, which is much closer to the architecture direction I want for the project.

This PR continues that direction by pushing the codebase further toward feature-based organization, explicit use-case handlers, and record-based contracts. It also reflects the fact that I’m already treating CQRS selectively, not as a blanket rule, with read-heavy and orchestration-heavy flows getting their own shape when that makes the code clearer.

The earlier ADR established the target state: Clean Architecture as the default structure, and CQRS where it earns its keep. This ADR is the practical next step from that foundation, not a separate architectural detour.

## Decision

I refactored the solution further around feature boundaries and cleaner use-case flow.

The main decisions were:

- Keep organizing code by feature instead of by generic technical buckets.
- Use records and `init`-only members more consistently for DTOs, requests, and results.
- Push more request handling through MediatR-style commands, queries, and handlers.
- Keep the command side focused on intent and orchestration, while persistence and external integration stay behind abstractions.
- Use projection-oriented storage for read-side concerns where that gives a clearer shape than a generic repository.
- Clean up older abstractions and naming so the code better reflects the actual domain and application flow.

## Rationale

This builds directly on the **Clean Architecture**/**CQRS** direction already documented in the earlier ADR. That document defined the target shape; this PR is about making the code actually look and behave that way.

The benefit of this approach is that the **application layer becomes easier to read** as a set of use cases, rather than a collection of incidental plumbing. It also **makes the read/write split more obvious**, which matters a lot in a system that uses **Cosmos DB**, **projections**, **durable orchestration**, and background workflows.

I also want the solution structure to **reduce cognitive load**. If a type is part of posts, reactions, thumbnails, or users, it should live with that feature as much as possible instead of being buried in a generic folder that hides its purpose.

## Consequences

### Positive

- The codebase is easier to navigate by feature.
- The intent of commands and queries is more obvious.
- The application layer is closer to the architectural boundary I want.
- Read-side and write-side concerns are more separated.
- The solution is easier to evolve without adding more generic abstractions.

### Negative

- A lot of namespaces, contracts, and references have to move at once.
- Some early model choices and schema decisions had to be cleaned up or replaced.
- There is more refactoring churn than in a smaller incremental change.
- Any code still depending on the older generic structure needs to be updated carefully.

## Alternatives considered

I could have left the existing shared structure in place and only made small incremental fixes. That would have been less disruptive, but it would also have preserved the architectural drift that was already making the solution harder to reason about.

I also could have treated CQRS as only a theoretical pattern and kept the application layer more repository-driven. I don’t think that would fit this codebase well anymore, because the photo, thumbnail, projection, and orchestration flows are already naturally moving toward command/query separation.

Another option would have been to keep all persistence concerns behind one broad repository abstraction. I’ve moved away from that because it hides too much and makes the read/write intent less explicit than I want.

## Result

The project is now more aligned with the architecture direction established in the earlier ADR. The solution has a clearer feature-based shape, the command side is more intent-driven, and the read side is moving toward explicit projections and storage abstractions where that improves clarity.

This is not a new direction so much as a more concrete implementation of the one I already committed to in the previous ADR.
