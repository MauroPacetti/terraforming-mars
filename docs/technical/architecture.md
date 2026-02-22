# Architettura Tecnica

---

## 1. Stack Tecnologico

### Engine
- **Unity 2022/2023 LTS** con URP (Universal Render Pipeline)
- Supporto nativo VR tramite OpenXR
- XR Interaction Toolkit per interazioni VR

### Networking
- Server autoritativo (server unico persistente)
- Netcode for GameObjects (Unity) o Mirror
- Spatial partitioning per gestione aree

### Backend
- Server dedicati per persistenza mondo
- Database per stato del mondo, giocatori, toponomastica
- Sistema di streaming terreno server-side
- API per dati NASA/ESA

### VR
- OpenXR come standard
- Supporto: Meta Quest, SteamVR, PCVR
- Fallback desktop (mouse + tastiera)

---

## 2. Architettura del Mondo

### Marte Persistente (Server Unico)
```
┌─────────────────────────────────────────────┐
│              MARS SERVER                     │
│                                              │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  Terrain  │  │  Struct  │  │  Players │  │
│  │  Streamer │  │  Manager │  │  Manager │  │
│  └──────────┘  └──────────┘  └──────────┘  │
│                                              │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  Energy   │  │  Naming  │  │    AI    │  │
│  │  Grid     │  │  Layer   │  │  Engine  │  │
│  └──────────┘  └──────────┘  └──────────┘  │
│                                              │
│  ┌──────────────────────────────────────┐   │
│  │        Persistent World State        │   │
│  │        (Database + Snapshots)        │   │
│  └──────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
```

### Client
```
┌─────────────────────────────────────────────┐
│              MARS CLIENT                     │
│                                              │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  Terrain  │  │ Panoptic │  │   VR /   │  │
│  │  LOD      │  │  Module  │  │  Desktop │  │
│  └──────────┘  └──────────┘  └──────────┘  │
│                                              │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │  Rover   │  │    AI    │  │   Net    │  │
│  │  Control │  │  Client  │  │  Sync    │  │
│  └──────────┘  └──────────┘  └──────────┘  │
└─────────────────────────────────────────────┘
```

---

## 3. Streaming del Terreno

### Pipeline
```
NASA Data (offline) → Tile Preprocessor → Tile Server
                                              ↓
                                         CDN / Cache
                                              ↓
                                         Client LOD Manager
                                              ↓
                                         Unity Terrain
```

### LOD Strategy
- LOD 0: MOLA globale (sempre in memoria, ~2GB)
- LOD 1: CTX regionale (streaming, ~50MB per regione)
- LOD 2: HiRISE locale (on-demand, ~500MB per tile)
- LOD 3: Procedurale (generato client-side)

### Formato Tile
- Heightmap: 16-bit grayscale PNG o raw
- Texture: ASTC/BC7 compressed
- Metadata: JSON (coordinate, elevazione min/max)
- Tile size: 256x256 o 512x512

---

## 4. Sistema Energetico (Backend)

### Simulazione
- Tick rate: 1 Hz (un aggiornamento al secondo)
- Griglia energetica: grafo di nodi e connessioni
- Ogni nodo: produzione, consumo, accumulo
- Simulazione fisica semplificata per trasporto

### Persistenza
- Stato energetico salvato ogni 30 secondi
- Snapshot completo ogni 5 minuti
- Log transazioni per audit e replay

---

## 5. IA Custode (Backend)

### Architettura
```
Behavior Logger → Pattern Detector → Context Builder → LLM → Response Filter → Client
```

### Componenti
- **Behavior Logger**: registra azioni significative
- **Pattern Detector**: identifica tendenze comportamentali
- **Context Builder**: costruisce prompt contestualizzato
- **LLM**: genera risposte (API esterna o modello dedicato)
- **Response Filter**: rate limiting, coerenza, sicurezza

### Storage
- Vector database per memoria a lungo termine
- Event store per cronologia azioni
- Player profile persistente (sopravvive alla morte)

---

## 6. Struttura Cartelle Unity

```
Assets/
  MarsPrototype/
    Scenes/
    Scripts/
      Module/          — Panopticon builder e logica capsula
      Terrain/         — Generazione e streaming terreno
      Rover/           — Controllo rover e droni
      Input/           — Input desktop e VR
      Energy/          — Sistema energetico client
      AI/              — Client IA custode
      Network/         — Networking e sincronizzazione
      Naming/          — Sistema toponomastico
      UI/              — Interfaccia utente
    Materials/
    Textures/
    Prefabs/
    Data/              — Configurazioni, tabelle costi
    Shaders/           — Shader custom per Marte
```

---

## 7. Requisiti Hardware (Target)

### Minimo (Desktop)
- CPU: 4 core, 3.0 GHz
- RAM: 8 GB
- GPU: GTX 1060 / RX 580
- Storage: 20 GB SSD
- Rete: 10 Mbps

### Consigliato (VR)
- CPU: 6 core, 3.5 GHz
- RAM: 16 GB
- GPU: RTX 3070 / RX 6800
- Storage: 50 GB SSD
- VR: Meta Quest 2/3, Valve Index, o equivalente
- Rete: 25 Mbps

---

## 8. Priorita' di Sviluppo

### Fase 1: Prototipo (Dove siamo)
- [x] Struttura progetto
- [ ] Capsula Panopticon (generazione procedurale)
- [ ] Terreno da heightmap
- [ ] Rover click-to-move
- [ ] Camera interna capsula

### Fase 2: Core Mechanics
- [ ] Sistema energetico base
- [ ] Costruzione modulare
- [ ] Drone di esplorazione
- [ ] IA custode (prototipo)

### Fase 3: Multiplayer
- [ ] Server persistente
- [ ] Sincronizzazione stato
- [ ] Chat / radio
- [ ] Interazioni tra giocatori

### Fase 4: Marte Reale
- [ ] Importazione dati MOLA
- [ ] Streaming LOD
- [ ] Toponomastica
- [ ] Coordinate reali

### Fase 5: VR
- [ ] XR Origin setup
- [ ] Interazione VR dalla capsula
- [ ] Comfort settings
- [ ] Haptic feedback
