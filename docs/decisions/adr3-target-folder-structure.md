# Clean Architecture Project Structure
The following is a suggested folder structure for a clean architecture project. This structure separates concerns and promotes
maintainability, testability, and scalability.


CloudCanvas.Application
├── Behaviors
├── Common
│   ├── Constants
│   ├── Exceptions
│   ├── Interfaces
│   │   ├── IMediaStorageService.cs
│   │   ├── ICurrentUserService.cs
│   │   └── IDateTimeProvider.cs
│   ├── Mapping
│   └── Validation
├── Photos
│   ├── Commands
│   │   ├── CreatePhoto
│   │   │   ├── CreatePhotoCommand.cs
│   │   │   ├── CreatePhotoCommandHandler.cs
│   │   │   ├── CreatePhotoResult.cs
│   │   │   └── CreatePhotoValidator.cs
│   │   └── UpdatePhoto
│   ├── Interfaces
│   │   └── IPhotoRepository.cs
│   ├── Queries
│   └── DTOs
├── Posts
│   ├── Commands
│   ├── Interfaces
│   │   ├── IPostRepository.cs
│   │   └── ICommentRepository.cs
│   ├── Queries
│   └── DTOs
├── Galleries
├── Reactions
└── Users