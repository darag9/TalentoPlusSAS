# 🏢 TalentoPlusSAS - Sistema de Gestión de Nómina y RRHH

Sistema backend robusto para la gestión de nómina, cálculo de KPIs y generación automática de certificados, desarrollado con **.NET 8** y arquitectura basada en **Microservicios (Simulados)** y **Domain-Driven Design (DDD)**.

Este proyecto implementa una solución completa conteinerizada con Docker, integrando Inteligencia Artificial, generación de PDFs y seguridad con JWT.

---

## 🚀 Tecnologías y Arquitectura

* **Lenguaje:** C# (.NET 8)
* **Arquitectura:** Domain-Driven Design (DDD) con separación de capas (Api, Application, Domain, Infrastructure).
* **Base de Datos:** PostgreSQL (Ejecutado en Docker).
* **ORM:** Entity Framework Core (Code-First).
* **Contenerización:** Docker & Docker Compose.
* **Documentación API:** Swagger / OpenAPI.
* **Seguridad:** ASP.NET Core Identity + JWT (JSON Web Tokens).
* **IA:** Integración con OpenAI (o simulación de servicio) para análisis de datos.
* **Utilidades:**
    * `ExcelDataReader`: Importación masiva de datos.
    * `QuestPDF`: Generación de reportes PDF de alto rendimiento.

---

## 🛠️ Instalación y Ejecución

El proyecto está diseñado para funcionar inmediatamente con **Docker**. No necesitas instalar PostgreSQL ni configurar cadenas de conexión manualmente.

### Prerrequisitos
* Docker Desktop (o Docker Engine en Linux).
* Git.

### Pasos para desplegar

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/tu-usuario/TalentoPlusSAS.git](https://github.com/tu-usuario/TalentoPlusSAS.git)
    cd TalentoPlusSAS
    ```

2.  **Configurar Variables de Entorno:**
    Crea un archivo `.env` en la raíz (basado en el ejemplo) con tus credenciales:
    ```env
    POSTGRES_USER=postgres
    POSTGRES_PASSWORD=TuPasswordSeguro
    POSTGRES_DB=TalentoPlusDB
    JWT_KEY=EstaEsUnaClaveSuperSecretaParaFirmarTokens123!
    OPENAI_API_KEY=sk-... (Opcional)
    ```

3.  **Levantar la Infraestructura:**
    Ejecuta el siguiente comando en la raíz del proyecto:
    ```bash
    docker compose up --build -d
    ```

4.  **Acceder al Sistema:**
    Una vez finalice, abre tu navegador en:
    👉 **http://localhost:8080/swagger**

---

## 📖 Guía de Uso (Flujo Principal)

Para probar la funcionalidad completa, sigue este orden en Swagger:

### 1. 📥 Importación Masiva (Pilar 1)
* **Endpoint:** `POST /api/Empleados/importar`
* **Acción:** Sube el archivo `Empleados.csv` o `.xlsx`. El sistema detectará automáticamente el formato, validará los datos y los persistirá en PostgreSQL.

### 2. 🔐 Seguridad y Registro (Pilar 3)
* **Endpoint:** `POST /api/Auth/register`
* **Regla de Negocio:** Solo puedes registrar usuarios cuyo `Documento` ya exista en la nómina importada en el paso anterior.
* **Endpoint:** `POST /api/Auth/login`
* **Acción:** Inicia sesión para obtener tu **Token JWT**.

### 3. 📊 Dashboard e IA (Pilar 2)
* **Endpoint:** `GET /api/Dashboard/kpis`
* **Acción:** Visualiza métricas en tiempo real (Total empleados, Vacaciones, Salarios).
* **Endpoint:** `POST /api/Dashboard/consulta-ia`
* **Acción:** Pregunta en lenguaje natural (ej: *"¿Cuál es el salario promedio?"*) y recibe respuestas basadas en tus datos.

### 4. 📄 Generación de Documentos (Pilar 1.3)
* **Endpoint:** `GET /api/Empleados/descargar-cv/{documento}`
* **Acción:** Genera y descarga instantáneamente una Hoja de Vida en formato PDF diseñada profesionalmente.

---

## ✒️ Autor
Desarrollado como prueba técnica de Arquitectura de Software.
