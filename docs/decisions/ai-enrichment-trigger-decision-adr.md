# ADR: Trigger Choice for AI Photo Enrichment in CloudCanvas

## Status

Proposed

## Context

CloudCanvas is adding an AI‑driven photo enrichment capability that analyzes images (tags + captions) using Azure AI Vision and persists the results back into the existing Cosmos DB photo documents via PATCH operations. The photo document must already exist in Cosmos DB before enrichment runs, because the AI metadata (`ai-tags`, `ai-caption`, etc.) is an augmentation of the existing projection, not a separate record.

The new Python‑based enrichment logic can be hosted in Azure Functions. The open question is: **which trigger should invoke the enrichment function** in a way that balances:

- Loose coupling between CloudCanvas and the enrichment function.
- CloudCanvas’ ability to control *when* enrichment happens.
- Clear, testable behavior for learning (AI‑200) and debugging.
- Operational robustness and future extensibility.

Four trigger candidates were considered:

1. HTTP trigger
2. Blob Storage trigger
3. Service Bus trigger
4. Cosmos DB Change Feed trigger

## Options Considered

### Option 1 – HTTP Trigger

**Description**
Expose an HTTP endpoint (e.g., `POST /photos/{photoId}/enrich`) implemented as an HTTP‑triggered Azure Function in Python. The function:

- Receives a photo ID.
- Loads the corresponding photo document from Cosmos DB.
- Calls Azure AI Vision to analyze the image.
- PATCHes AI metadata fields into the document.

**Pros**

- Very explicit API surface; easy to call from Postman, scripts, or CloudCanvas backend.
- Straightforward to debug and iterate: request → response, with clear logs.
- Great for early development and AI‑200 learning around HTTP triggers and bindings.

**Cons**

- CloudCanvas must explicitly call the function (direct HTTP), creating tighter coupling between the app and the enrichment implementation.
- Less “serverless/event‑driven” feel; enrichment becomes a synchronous or semi‑synchronous operation invoked by CloudCanvas code, not a background pipeline.

**Decision in evaluation**
Kept as **Phase 1 / development‑only trigger** (to bootstrap the enrichment logic and test flows), but **not chosen as the long‑term production trigger** because it couples CloudCanvas directly to the enrichment endpoint and does not fully leverage event‑driven patterns.

***

### Option 2 – Blob Storage Trigger

**Description**
Use a Blob Storage trigger on the thumbnail or photo container. When a blob is created/updated, the function:

- Reads blob metadata or naming convention to derive the photo ID.
- Loads or creates the corresponding photo document.
- Calls Azure AI Vision.
- PATCHes AI metadata fields into the document.

**Pros**

- Classic serverless pattern: “when a file arrives, process it.”
- Fully decoupled from CloudCanvas’ backend logic; enrichment is driven by blob events.
- Matches Azure reference architectures for image processing pipelines.

**Cons**

- Timing is implicit: enrichment fires when the blob appears, not necessarily when the photo document is fully initialized in Cosmos DB.
- Requires careful coordination: CloudCanvas must ensure the Cosmos document exists with the right fields before or at the same time as blob creation, or the function must handle “document not yet present” gracefully.
- Adds complexity when the requirement is “only PATCH existing documents” rather than creating them.

**Decision in evaluation**
Eliminated as a **primary trigger** for this specific use case, because the requirement that the Cosmos document must exist before enrichment adds coordination complexity. Blob trigger is still a useful pattern for other file‑processing workloads, but not the best fit for “only PATCH existing photo projections.”

***

### Option 3 – Service Bus Trigger

**Description**
CloudCanvas publishes a message (queue or topic) when a photo is “ready for enrichment” (e.g., after blob upload and Cosmos document creation). An Azure Function with a Service Bus trigger:

- Consumes the message.
- Loads the photo document from Cosmos DB.
- Calls Azure AI Vision.
- PATCHes AI metadata into the document.

**Pros**

- Loose coupling: the enrichment function only knows about Service Bus and Cosmos, not CloudCanvas internals.
- CloudCanvas remains in control of *when* enrichment happens by deciding when to publish messages (only after the document is fully initialized and thumbnails/URLs are present).
- Strong operational story: DLQ, retries, and observability are well‑supported with Service Bus.
- Familiar pattern, given existing experience with Service Bus‑triggered functions.

**Cons**

- Requires a messaging layer that must be configured and maintained (namespaces, queues/topics, access, monitoring). Luckily, CloudCanvas has already fulfilled this requirement by using Durable Function orchestration for thumbnail generation. 
- Slightly more moving parts compared to a purely data‑driven trigger like change feed.
- Requires synchronous CloudCanvas code to emit messages as part of the write flow, which is an extra step compared to “just writing to Cosmos.”

**Decision in evaluation**
Kept as one of the **final two candidates**, because it offers a good balance between **loose coupling** and **explicit timing control**: CloudCanvas decides exactly when enrichment should occur by sending a message only when both blob and Cosmos document are in a consistent state.

***

### Option 4 – Cosmos DB Change Feed Trigger

**Description**
Use the Cosmos DB change feed trigger on the photo container. When a photo document is inserted or updated, the function:

- Receives the changed document from the change feed.
- Determines whether it is ready for enrichment (e.g., thumbnails URLs present, domain flags set).
- Calls Azure AI Vision.
- PATCHes AI metadata into the document.

**Pros**

- Strong loose coupling: CloudCanvas only writes documents; the function reacts to changes without explicit calls or messages.
- Natural match to “only enrich existing documents,” since the change feed emits inserts/updates, not deletes.
- Leverages Cosmos DB’s built‑in scaling and checkpointing mechanisms for the change feed.
- Good learning opportunity and aligns with exam topics around triggers, bindings, and data‑driven event processing.

**Cons**

- Timing is more implicit: any qualifying write can trigger enrichment, so more logic is needed to:
  - Ensure enrichment only runs once (or when appropriate).
  - Avoid re‑enrichment on every unrelated update.
- Error handling and retry strategies must be carefully designed (e.g., DLQ pattern) to avoid getting stuck on problematic documents.
- Requires a clear rule set to decide *which* changes are “ready for enrichment” (e.g., “thumbnails URLs present” flag) instead of relying on an explicit “enrich now” message.

**Decision in evaluation**
Kept as one of the **final two candidates**, primarily because it combines loose coupling with a strong data‑driven model and offers a bigger learning opportunity around change feed processing.

***

## Decision Outcome (Conditional)

The final decision between **Service Bus trigger** and **Cosmos DB change feed trigger** is intentionally **conditional**, based on one key technical question:

> Can the Cosmos DB change feed trigger be reliably configured/implemented so that enrichment only fires when the photo document has its thumbnail URLs (or equivalent “ready” signals) set?

- **If yes** (i.e., it is straightforward to filter change feed events based on document content and only enrich when thumbnails or readiness flags are present), then:
  - **Preferred choice**: **Cosmos DB change feed trigger**
    - Reason: maintains loose coupling and uses a purely data‑driven pattern, while still letting CloudCanvas “control timing” indirectly by controlling when and how it writes documents. It also provides a richer learning surface for AI‑200 and future event‑driven designs.
- **If no** (i.e., reliably gating enrichment on thumbnails/readiness proves too complex or brittle), then:
  - **Preferred choice**: **Service Bus trigger**
    - Reason: keeps the enrichment function decoupled but gives CloudCanvas **explicit, deterministic control** over timing via messages (“ready for enrichment” events) that are only published after the photo document and blob are in a consistent state. This makes the behavior easier to reason about and debug operationally.

The ADR will be updated to **Accepted** once this gating behavior is validated in a small prototype (checking whether the change feed trigger can reliably act only after thumbnails/URLs are present). If the prototype shows this is cleanly achievable, the decision will lean toward change feed; otherwise, Service Bus will be adopted as the primary production trigger.

## Consequences

- All four triggers (HTTP, Blob, Service Bus, Cosmos DB change feed) were explicitly considered and documented, reflecting the actual decision‑making journey rather than only the final result.
- Short‑term development will likely use an HTTP‑triggered function for ease of testing and iteration, regardless of the final production trigger choice.
- The final trigger will be either Service Bus or Cosmos DB change feed, depending on the outcome of a targeted prototype around readiness gating in Cosmos.
- Future ADRs may refine this decision (for example, introducing a hybrid pattern where change feed writes to Service Bus for complex workflows, following recommended patterns that pair change feed with messaging for robust error handling and integration).

***


