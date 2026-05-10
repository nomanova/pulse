# Pulse ⚡

### Self-hosted notification server for .NET

> **🚧 This project is in early development.**
> The core is being built. If the value proposition below resonates with you, we'd love to hear from you — see [Get Involved](#get-involved).

---

## The problem

Every .NET application eventually needs to send notifications — email, SMS, in-app, push, Slack, Teams... The options today are:

- **Build it yourself.** Weeks of boilerplate, and you'll rebuild the same retry logic, provider abstraction, and user preference management every time.
- **Use a hosted SaaS** Using Knock, Courier, or SuprSend... Fast to start, but your notification data lives on someone else's servers — a non-starter for regulated industries.
- **Self-host** Using Novu for example: but it's Node.js, requires MongoDB, runs six separate services, and the self-host experience is underwhelming.

If you run a .NET shop in transportation, fintech, healthcare, legal tech, or any regulated industry, none of these options are good enough.

---

## What Pulse is

Pulse is an open-core notification server built from the ground up for self-hosted, production .NET deployments.

**First-class self-hosting.** Not an afterthought. A single `docker compose up` that works the first time. Production-ready Helm charts. Clear documentation that covers backups, upgrades, scaling, and monitoring — without hiding anything to nudge you toward a cloud tier.

**Built for .NET teams.** ASP.NET Core throughout. PostgreSQL or SQL Server. Native Azure integration including Entra ID, Azure Service Bus, and Azure Container Apps. An idiomatic C# SDK that feels like it belongs in your codebase.

**Multichannel workflow engine.** Define notification workflows in code or via a visual editor. Email, SMS, in-app, push, Slack, Microsoft Teams, and WhatsApp — with intelligent fallback routing, digest batching, and per-user preference management.

**Provider abstraction.** Swap SendGrid for Mailgun, Twilio for Vonage, without touching your application code. Rate limit handling, retry logic, and delivery tracking are handled for you.

**Open-core.** The core is AGPLv3 — free to use, self-host, and modify. Enterprise features (SSO/SAML, RBAC, audit logs, data retention policies, SLA-backed support) are available under a commercial license.

---

## Who this is for

- **.NET engineering teams** building SaaS products who need notifications to work reliably without building everything from scratch
- **Regulated industries** where data residency requirements make hosted SaaS non-negotiable
- **Microsoft-stack organizations** who want infrastructure that integrates naturally with Azure, Entra ID, and the broader .NET ecosystem

---

## Status

The project is currently in the design and early build phase. Here is what is planned for v1:

- [ ] Core workflow engine with trigger API
- [ ] Email and SMS channels
- [ ] PostgreSQL persistence
- [ ] Single-service Docker Compose deployment
- [ ] Basic management dashboard
- [ ] C# SDK (NuGet)
- [ ] Production deployment documentation

In-app, push, Slack, Teams, and additional providers will follow in later releases.

---

## Licence

The core of Pulse is licensed under the **GNU Affero General Public Licence v3.0 (AGPLv3)**. You are free to self-host, use, and modify it.

If you offer Pulse as a service or need license terms that better fit your organization's policies, a commercial license is available — [get in touch](#get-involved).

---

## Get Involved

The project is at an early stage, and the roadmap is being shaped right now. If any of the following apply to you, reaching out would be genuinely valuable:

- You've tried to self-host other notification infrastructure products and hit walls
- You're a .NET team in a regulated industry looking for exactly this kind of solution
- You have opinions on what a production-grade self-hosted deployment experience should look like
- You'd like to be an early design partner or beta tester

**→ Open a Github issue or discussion to share your use case or thoughts**

**→ Or email directly: info \_at\_ nomanova \_dot\_ com**

No sales process. No pitch deck. Just an honest conversation about whether this solves a real problem for you.
