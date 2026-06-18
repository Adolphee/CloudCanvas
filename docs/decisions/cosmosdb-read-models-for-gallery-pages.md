# ADR-NNN: Use Cosmos DB Read Models for Gallery Page Rendering

## Status

Accepted

## Date

2026-06-17

## Context

The application stores uploaded files in Azure Blob Storage and maintains relational data in SQL Server for users, galleries, comments, likes, and related entities. The gallery page is expected to become increasingly read-heavy as features expand to include many images, nested comment threads, reactions on posts, and reactions on comments.

Rendering this page directly from the relational model would likely require increasingly complex queries, multiple joins, and larger object graphs in the application layer. This would increase query cost, API response time, and implementation complexity for page-specific read scenarios.

The system also needs to preserve a reliable transactional source of truth while supporting fast page rendering for a social-media-style experience.

## Decision

Keep SQL Server as the transactional source of truth and Blob Storage as the file store, and introduce Cosmos DB as a read-model store for gallery page rendering.

Azure Functions orchestration will project relational data into page-specific, denormalized documents in Cosmos DB. These documents will be shaped around the gallery page use case rather than mirroring full SQL entity graphs.

The Cosmos DB documents will contain only the data required to render the page efficiently, such as gallery metadata, lightweight author information, image URLs, reaction counts, and a bounded subset of recent or paged comments.

## Rationale

This decision adopts a CQRS-style separation between the write model and the read model. SQL Server remains optimized for consistency, relationships, and transactional updates, while Cosmos DB is optimized for fast retrieval of a page-shaped document.

The main alternative considered was continuing to serve the gallery page directly from SQL Server through Entity Framework queries and joins. That approach keeps the architecture simpler in the short term, but it becomes harder to scale as the gallery page accumulates more related data and more nested UI requirements.

Another alternative considered was copying full relational entity graphs into Cosmos DB, for example embedding structures such as `Post > User > Address`. That option was rejected because it would duplicate unnecessary data, increase document size, create update pressure when shared entities change, and blur the boundary between transactional entities and read models.

A page-specific read model was chosen because it keeps the Cosmos projection focused on query needs rather than on entity fidelity. It also allows selective denormalization, such as embedding lightweight author summaries and recent comments, without overcommitting to full object mirroring.

## Consequences

**Easier:**
- Fast gallery page reads with fewer round-trips and less server-side join complexity.
- Read models can be shaped exactly for UI needs, which simplifies API response construction.
- SQL Server remains cleanly positioned as the source of truth for transactional operations.
- The architecture aligns with CQRS and can scale to more specialized read views later.

**Harder:**
- The system must accept eventual consistency between SQL writes and Cosmos DB projections.
- Projection logic, orchestration, retries, and rebuild workflows must be implemented and maintained.
- Poorly designed documents may become too large or too frequently updated for hot galleries.
- Comment threads and high-churn reaction data may require separate projections or paging strategies rather than one fully embedded document.
