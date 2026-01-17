# DepoX Architecture

## Overview

DepoX is a .NET MAUI application designed for warehouse handheld and desktop usage.
It follows an offline-first approach with explicit online commit behavior.

The application itself does not make stock decisions.
All critical business rules (FIFO, stock validation) are owned by ERP.

---

## Core Rules (Non-Negotiable)

- ERP integration is via **SOAP services**
- FIFO calculation is performed **only by ERP**
- The mobile application never recalculates FIFO
- Offline operations are stored locally as drafts
- Online operation is explicitly triggered by user action (**Kaydet**)
- If online save fails, local data remains and retry is possible
- Barcode input supports **camera and keyboard simultaneously**

---

## Offline / Online Transaction Model

### OfflineEkle (Draft)
- Can be executed without network
- Stored in SQLite
- Can be edited and continued later
- Represents *intent*, not final ERP transaction

### Kaydet (Online Commit)
- Requires active network
- Sends data to ERP via SOAP
- ERP validates stock, FIFO, and consistency
- On success:
  - Local draft is marked as completed
- On failure:
  - Draft remains
  - Error is stored
  - Retry is allowed

---

## ERP Integration Strategy

- ERP access is isolated behind a gateway interface
- Application layer depends on abstractions only
- SOAP implementation details must not leak into UI or ViewModels

Example abstraction:
- IErpGateway
- SoapErpGateway

---

## Application Layers (Target State)

- UI (MAUI Views)
- ViewModels (MVVM)
- Application Services (Use cases)
- Domain Models
- Infrastructure
  - SOAP
  - SQLite
  - Sync logic

---

## Sync Responsibility

- Application controls *when* to sync
- ERP controls *what is valid*
- Sync service:
  - Sends pending drafts
  - Handles retries
  - Stores last error state
  - Never silently drops data

---

## Guiding Principle

> The mobile app assists warehouse operators.  
> The ERP system remains the single source of truth.
