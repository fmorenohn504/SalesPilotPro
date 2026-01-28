# SalesPilotPro – Backend (.NET)

Backend del sistema **SalesPilotPro**, diseñado con una arquitectura modular,
multi-tenant y desacoplada del frontend (Flutter).

Este repositorio contiene **únicamente el backend**.

---

## Arquitectura

Estructura del proyecto:

    src/
     ├── Api/                → ASP.NET Core API
     ├── Core/               → Dominio y contratos
     └── Infrastructure/     → Implementaciones técnicas

---

## Ejecución local

Comandos:

    dotnet restore
    dotnet run --project src/Api/SalesPilotPro.Api

Por defecto:

    https://localhost:5001

---

## Comunicación con Flutter

- Flutter consume esta API vía HTTP REST
- Autenticación basada en JWT
- Flutter no contiene lógica de negocio

---

## Notas

- Repositorio solo backend
- Frontend Flutter vive en un repositorio separado
