***

## CloudCanvas.Shared

**CloudCanvas.Shared** is the shared kernel of the CloudCanvas platform: a durable substrate of contracts, abstractions, and helper services that sit between independently deployed Azure Function apps. It captures the cross-cutting concerns of an orchestration‑aware, event‑driven system so each function app can focus on its own business workflow instead of re‑implementing plumbing.

### What this package is for

Use **CloudCanvas.Shared** when you want:

- A **single source of truth** for DTOs and contracts exchanged between your function apps (for example: gallery items, blob metadata, Cosmos documents).  
- **Cloud‑agnostic interfaces** for Blob Storage, Service Bus, and Cosmos DB, plus concrete wrappers that apply consistent configuration (retry, naming, serialization, message shape).  
- A small set of **utility services** (message builders, patch operation builders, serializers, validation helpers) that make your orchestrations and activities easier to read and test.

This package is not a general‑purpose cloud SDK wrapper; it is the backbone of CloudCanvas’ own architecture and is optimized for durable, event‑driven gallery workflows.

### Main building blocks

You can describe the folders like this (matching what’s in your screenshot):

- **DTOs**  
  Immutable models for messages and documents shared across services (blob metadata, gallery items, Cosmos document base types, patch DTOs). These are the shapes that travel across queues, HTTP calls, and persistence boundaries.

- **Interfaces**  
  Contracts such as `IBlobStorageService`, `ICosmosClientWrapper`, `IServiceBusAdapter`, `IServiceBusMessageBuilder`, `IValidationTool`, which hide Azure SDK details behind intention‑revealing methods.

- **Services**  
  Opinionated implementations of the interfaces: Blob Storage operations, Cosmos access, Service Bus client and message factory, plus adapters that apply consistent options and diagnostics.

- **Utilities**  
  Helpers like serializers, image tools, and patch operation builders to keep orchestration and activity functions lean.

- **Exceptions & Enums**  
  Domain‑specific exceptions and enums that give function apps a common vocabulary for error handling and routing decisions.

### Getting started

1. **Install the package**

```powershell
dotnet add package CloudCanvas.Shared
```

2. **Register services in your Functions host**

In your Functions startup/`Program.cs`, register the shared services and adapters into the DI container, alongside the Azure clients they depend on. (Here you’d show a minimal example of adding `IBlobStorageService`, `IServiceBusAdapter`, etc.)

3. **Consume abstractions in your functions**

Inject the interfaces into orchestrations, activities, or plain functions:

```csharp
public class ExtractMetadata
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IValidationTool _validation;

    public ExtractMetadata(IBlobStorageService blobStorage, IValidationTool validation)
    {
        _blobStorage = blobStorage;
        _validation = validation;
    }

    [Function("ExtractMetadata")]
    public async Task Run([BlobTrigger("...")] Stream blob, string name)
    {
        var metadata = await _blobStorage.ReadMetadataAsync(name);
        _validation.EnsureValid(metadata);
        // ...
    }
}
```

The function code stays focused on gallery logic; cross‑cutting storage and validation concerns live in CloudCanvas.Shared.

### Design philosophy

CloudCanvas.Shared is built around a few principles:

- **Separation through precision** – each interface does one narrow thing; every DTO is explicit about what it represents.  
- **Cooperation through contracts** – independently deployed functions agree on DTOs and interfaces rather than implicit conventions.  
- **Orchestration‑aware** – abstractions play nicely with Durable Functions patterns, idempotency, and message replay.

### Feedback and contributions

If you’re experimenting with CloudCanvas or have ideas for new shared abstractions, open an issue or a discussion in the repository. Feedback on new DTOs, service boundaries, or Azure resource patterns is especially welcome.

***