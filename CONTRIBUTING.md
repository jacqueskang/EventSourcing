# Contributing to EventSourcing

Thank you for your interest in contributing to this project! This guide will help you get started.

## Table of Contents

- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Code Guidelines](#code-guidelines)
- [Submitting Changes](#submitting-changes)
- [Release Process](#release-process)

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Git
- Your favorite IDE (Visual Studio, VS Code, Rider)

### Setting Up Your Environment

1. **Fork the repository** on GitHub

2. **Clone your fork**:
   ```bash
   git clone https://github.com/YOUR-USERNAME/EventSourcing.git
   cd EventSourcing
   ```

3. **Add upstream remote**:
   ```bash
   git remote add upstream https://github.com/jacqueskang/EventSourcing.git
   ```

4. **Restore dependencies**:
   ```bash
   dotnet restore src/EventSourcing.sln
   ```

5. **Build the solution**:
   ```bash
   dotnet build src/EventSourcing.sln
   ```

6. **Run tests**:
   ```bash
   dotnet test src/EventSourcing.sln
   ```

## Development Workflow

### 1. Create a Feature Branch

Always create a new branch for your work:

```bash
git checkout develop
git pull upstream develop
git checkout -b feat/your-feature-name
```

**Branch naming conventions**:
- `feat/` - New features
- `fix/` - Bug fixes
- `docs/` - Documentation changes
- `refactor/` - Code refactoring
- `test/` - Test additions or fixes
- `ci/` - CI/CD changes

### 2. Make Your Changes

- Write clean, readable code
- Follow existing code patterns and conventions
- Add tests for new functionality
- Update documentation as needed

### 3. Test Your Changes

```bash
# Run all tests
dotnet test src/EventSourcing.sln

# Run tests with coverage
dotnet test src/EventSourcing.sln --collect:"XPlat Code Coverage"
```

### 4. Commit Your Changes

Use [Conventional Commits](https://www.conventionalcommits.org/) format:

```bash
git add .
git commit -m "feat: add snapshot support for DynamoDB"
```

**Commit message format**:
```
<type>: <short summary>

<optional body>

<optional footer>
```

**Types**:
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation only
- `style:` - Formatting, missing semicolons, etc.
- `refactor:` - Code change that neither fixes a bug nor adds a feature
- `test:` - Adding or updating tests
- `ci:` - CI/CD changes
- `chore:` - Maintenance tasks

**Examples**:
```bash
git commit -m "feat: add CosmosDB snapshot store"
git commit -m "fix: resolve null reference in aggregate repository"
git commit -m "docs: update DynamoDB setup instructions"
```

### 5. Keep Your Branch Updated

```bash
git fetch upstream
git rebase upstream/develop
```

### 6. Push to Your Fork

```bash
git push origin feat/your-feature-name
```

## Code Guidelines

### Architecture Patterns

- **Aggregates**: Must implement `IAggregate<TKey>` and follow the event sourcing pattern
- **Events**: Must be immutable and JSON-serializable
- **Repositories**: Follow the `IAggregateRepository<TAggregate, TKey>` interface
- **Persistence providers**: Implement `IEventStore<TAggregate, TKey>` and optionally `ISnapshotStore<TAggregate, TKey>`

### Coding Conventions

- Use C# naming conventions (PascalCase for classes, camelCase for parameters)
- Keep methods focused and small
- Use async/await for I/O operations
- Include XML documentation comments for public APIs
- Follow existing patterns in the codebase

### Testing

- Write unit tests for all new functionality
- Use xUnit for test framework
- Test files should be in `*.Tests` projects
- Follow AAA pattern: Arrange, Act, Assert

Example test:
```csharp
[Fact]
public async Task DebitGiftCard_WithSufficientBalance_Succeeds()
{
    // Arrange
    var giftCard = new GiftCard(Guid.NewGuid(), 100m);
    
    // Act
    giftCard.Debit(50m);
    
    // Assert
    Assert.Equal(50m, giftCard.Balance);
}
```

## Submitting Changes

### Pull Request Process

1. **Ensure all tests pass** and code builds successfully

2. **Update documentation** if needed:
   - README.md for user-facing changes
   - XML comments for API changes
   - Setup guides in `doc/` folder for new features

3. **Create a Pull Request**:
   - Go to your fork on GitHub
   - Click "New Pull Request"
   - Set base branch to `develop`
   - Fill in the PR template

4. **PR Requirements**:
   - All tests must pass (enforced by CI)
   - Code coverage should not decrease significantly
   - At least one maintainer approval required
   - No merge conflicts with `develop`

### PR Title Format

Use the same format as commit messages:
```
feat: add Redis caching support
fix: resolve race condition in event store
docs: improve CosmosDB setup guide
```

### PR Description Template

```markdown
## Summary
Brief description of the changes

## Changes
- List of changes made
- Another change

## Testing
How to test these changes

## Related Issues
Closes #123
```

### What Happens Next?

1. **Automated checks run**: Build, test, coverage
2. **Code review**: Maintainers will review your PR
3. **Feedback**: Address any requested changes
4. **Approval**: Once approved, a maintainer will merge

## Release Process

Maintainers handle releases following this process:

### For Contributors

- You don't need to update version numbers
- Version bumps happen separately from feature PRs

### For Maintainers

1. **Update version** in `src/Directory.Build.props`:
   ```xml
   <Version>1.3.0</Version>
   ```

2. **Commit and merge to develop**:
   ```bash
   git commit -m "chore: bump version to 1.3.0"
   git push origin develop
   ```

3. **Create and push tag**:
   ```bash
   git tag v1.3.0
   git push origin v1.3.0
   ```

4. **GitHub Actions automatically**:
   - Builds packages
   - Creates GitHub release
   - Publishes to NuGet.org

### Versioning

This project follows [Semantic Versioning](https://semver.org/):
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

## Adding a New Persistence Provider

If you're adding a new storage backend:

1. Create `JKang.EventSourcing.Persistence.YourProvider` project
2. Implement `IEventStore<TAggregate, TKey>`
3. Optionally implement `ISnapshotStore<TAggregate, TKey>`
4. Add builder extension for DI configuration
5. Create initializer if backend needs setup
6. Add comprehensive tests
7. Create setup guide in `doc/YourProviderSetup.md`
8. Update main README.md

See existing providers (FileSystem, DynamoDB, CosmosDB) for reference patterns.

## Getting Help

- **Questions**: Open a [GitHub Discussion](https://github.com/jacqueskang/EventSourcing/discussions)
- **Bugs**: Open a [GitHub Issue](https://github.com/jacqueskang/EventSourcing/issues)
- **Review**: Tag `@jacqueskang` in your PR for review

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on the code, not the person
- Help others learn and grow

Thank you for contributing! 🎉
