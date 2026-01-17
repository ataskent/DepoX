# DepoX

DepoX is a warehouse handheld application built with .NET MAUI.

It runs on Android and Windows devices and is designed for real-world
warehouse operations such as counting, transfer, and shipment execution.

---

## Core Principles

- ERP integration is done via **SOAP services**
- **FIFO is calculated by ERP**, not by the mobile app
- Offline operations are stored locally as **Draft / OfflineEkle**
- Online operation is explicitly triggered by **Save (Kaydet)**
- If Save succeeds, local data is finalized
- If Save fails, the operation continues from where it stopped
- Barcode input supports **camera and keyboard simultaneously**

---

## Offline / Online Model

- **OfflineEkle**
  - Stores transactions locally in SQLite
  - Can be performed without network
- **Kaydet**
  - Requires network
  - Sends data to ERP via SOAP
  - On success: local record is marked as completed
  - On failure: data remains locally and can be retried

---

## Platforms

- Android (handheld terminals)
- Windows (tablet / desktop)

---

## Architecture Goals

- MVVM pattern
- Layered and testable design
- ERP-dependent logic isolated behind gateway interfaces
- Offline-first, sync-later approach

---

## Status

This project is under active development.
Architecture and core infrastructure are being stabilized.
