# WASender AI Marketing Features

This branch adds the conversational AI marketing layer for WASender, designed around the same automation and WhatsApp lead-generation workflows already used in the project.

## Included features

- Claude-style intent classification for customer conversations
- 24/7 lead qualification flows
- Order intake and status management workflows
- Appointment booking automation
- Follow-up and reminder paths
- A drag-and-drop bot studio to design and test conversational flows

## How to run the bot builder

Launch the application with the marketing bot option:

```bash
WASender.exe --marketing-bot
```

The built-in flow builder lets you drag steps like `Lead qualification`, `Appointment booking`, and `Human handoff` into a campaign canvas, then test the response against a sample customer message.

## Design goal

This module is intentionally lightweight and framework-compatible so it can coexist with the current WinForms WhatsApp automation app without replacing the core scheduling and browser automation features.
