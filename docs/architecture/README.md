# Architecture Documentation

This domain contains documentation about EasyKit's system structure, module boundaries, and integration points.

## Read When

- Understanding system structure -> `system.md`
- Adding a new tool/controller -> `modules.md`
- Security changes -> `security.md`
- Modifying process execution -> `security.md`

## Knowledge Units

| Document | Purpose |
|----------|---------|
| [system.md](system.md) | Overall system architecture and component relationships |
| [modules.md](modules.md) | Module boundaries and responsibilities |
| [security.md](security.md) | Security architecture and SecureProcessRunner design |

## Related Domains

- **Decisions:** Package manager selection, security architecture choices
- **Solutions:** Command injection prevention patterns
- **Constraints:** Windows-only platform, .NET 8.0 runtime
