# Media Journal

Media Journal è un'applicazione web per tenere traccia dei media consumati (libri, film, serie TV, videogiochi), completa di sistema di rating e recensioni. L'applicazione permette agli utenti di mantenere un diario personale delle proprie esperienze mediali, con la possibilità di condividere pubblicamente le proprie recensioni.

## Panoramica del Progetto

Media Journal è stato sviluppato come applicazione web moderna con funzionalità complete di Progressive Web App (PWA). Gli utenti possono:
- Registrare e valutare i media consumati
- Scrivere recensioni private o pubbliche
- Visualizzare statistiche dettagliate sul proprio consumo mediatico
- Accedere ai propri contenuti anche offline
- Installare l'applicazione come app nativa sui propri dispositivi

L'applicazione è pensata per appassionati di media che desiderano tenere traccia delle proprie esperienze e, opzionalmente, condividerle con altri utenti.

## Stack Tecnologico

### Backend
- ASP.NET Core MVC per l'architettura dell'applicazione
- Entity Framework Core con SQLite per la persistenza dei dati
- ASP.NET Core Identity per l'autenticazione e autorizzazione degli utenti

### Frontend
- Bootstrap 5 per un'interfaccia responsive e mobile-first
- Bootstrap Icons per l'iconografia
- JavaScript vanilla per le interazioni lato client e la data visualization

### Progressive Web App
- Service Worker per la gestione della cache e il supporto offline
- Web App Manifest per l'installazione come app
- Strategie di caching avanzate per contenuti statici e dinamici

## Caratteristiche Implementate

### Sistema di Autenticazione
- Registrazione e login utenti
- Gestione profilo
- Protezione delle route sensibili

### Gestione Media
- Creazione, modifica, visualizzazione e eliminazione dei media
- Supporto per diverse tipologie: libri, film, serie TV, videogiochi
- Sistema di rating da 1 a 10 stelle
- Possibilità di scrivere recensioni dettagliate
- Opzione per rendere pubbliche le recensioni

### Dashboard Interattiva
- Statistiche sul consumo mediatico
- Distribuzione dei rating visualizzata tramite grafico
- Conteggi per tipologia di media
- Media dei rating
- Lista delle attività recenti

### Sistema di Review Pubbliche
- Possibilità di rendere pubbliche le proprie recensioni
- Pagina dedicata alla visualizzazione delle recensioni pubbliche
- Filtri per tipo di media
- Ricerca in tempo reale

### Funzionalità Mobile e Offline
- Design responsive ottimizzato per dispositivi mobili
- Installabile come PWA su dispositivi supportati
- Funzionamento offline completo per:
  - Visualizzazione dei propri media
  - Accesso alle statistiche
  - Lettura delle recensioni pubbliche
- Sincronizzazione automatica al ripristino della connessione

## Architettura

### Pattern MVC
- Controllers per la logica di business
- Models per la rappresentazione dei dati
- Views per la presentazione
- ViewModels per dati specifici delle view

### Sicurezza
- CSRF protection tramite anti-forgery token
- Validazione form lato server tramite data annotations
- Validazione base HTML5 lato client
- Prevenzione SQL injection tramite Entity Framework
- Autorizzazione basata su [Authorize] attribute

### Progressive Web App
- Cache statica per assets (CSS, JS, immagini)
- Cache runtime per contenuti dinamici
- Strategia di caching offline-first
- Gestione sofisticata degli aggiornamenti cache

## Setup e Installazione

### Requisiti
- .NET 8.0 o superiore

### Configurazione Database
Il database SQLite verrà creato e configurato automaticamente al primo avvio dell'applicazione. Le migrazioni verranno applicate automaticamente.
### Avvio Applicazione

### Docker
L'applicazione può essere avviata facilmente usando Docker Compose:

- Clonare il repository
- Navigare nella directory del progetto
- Eseguire docker-compose up --build
- L'applicazione sarà disponibile su http://localhost:8080

Il database SQLite verrà persistito attraverso un volume Docker dedicato.

### IDE
L'applicazione può essere eseguita direttamente da Visual Studio o Rider:

- Aprire la solution in Visual Studio/Rider
- Ripristinare i pacchetti NuGet
- Premere il pulsante Run per avviare l'applicazione
- Il browser si aprirà automaticamente all'indirizzo configurato

Per entrambe le modalità di avvio, è possibile personalizzare la configurazione modificando il file appsettings.json.

## Funzionalità Mobile

L'applicazione è stata sviluppata seguendo un approccio mobile-first:
- Layout responsive che si adatta a tutti i dispositivi
- Interfaccia touch-friendly
- Installabile come app tramite PWA
- Funzionamento offline completo
- Service worker per gestione cache e aggiornamenti

L'implementazione PWA permette agli utenti di:
- Installare l'app sulla home screen
- Accedere ai contenuti offline
- Ricevere aggiornamenti automatici
