---
name: generate_documentation
description: Standards for creating project documentation (READMEs, ADRs, Release Notes) based on the comprehensive Cartolafy templates.
version: 1.0.0
---

# 📚 Generate Documentation

**Creates standardized, high-quality technical documentation matching the project's premium aesthetic.**

## 🎯 Scope
Use this skill when:
*   Creating a `README.md` for a new service/module.
*   Documenting an API Endpoint.
*   Writing Release Notes or Changelogs.

## 📝 README Standard

Every module must have a `README.md` following this structure (emojis included):

```markdown
# 📦 [Module Name]

**[One-line high-impact summary]**

## 📋 Overview
[Detailed description of the responsibilities of this module]

## 🛠️ Tech Stack
*   **Framework**: .NET 10
*   **Library**: [Key Libs]

## 🏗️ Architecture
Describes how this module fits into the Clean Architecture.

## 🚀 Getting Started
Steps to run/test this specific module.
```

## 🔌 API Endpoint Documentation

When documenting an Endpoint in Markdown:

```markdown
# Endpoint: [Verb] [Path]

## 📝 Overview
[Description]

## 🔒 Authentication
Requires: `[Role]`

## 📥 Request
```json
{
  "key": "value"
}
```

## 📤 Response (200 OK)
```json
{
  "id": 1,
  "key": "value"
}
```

## ⚠️ Error Responses
*   **400 Bad Request**: Validation failed.
*   **404 Not Found**: Resource not found.
```

## 📜 ADR (Architecture Decision Record)

When a major decision is made/proposed:

1.  **Context**: What is the problem?
2.  **Decision**: What are we doing?
3.  **Consequences**: Pros/Cons.
4.  **Alternatives**: What did we reject?

## ✅ Checklist for Docs
*   [ ] Use emojis to section headers (visual scanning).
*   [ ] Include code snippets for configuration.
*   [ ] Mention versioning compatibility.
*   [ ] Ensure `utf-8` encoding.
*   [ ] Link to related "Parent" documentation.
