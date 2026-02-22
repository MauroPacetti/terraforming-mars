# Marte Reale e Toponomastica Civile

> "Human Cultural Layering of Mars"

---

## 1. Visione

Il giocatore non esplora un mondo generato. Esplora Marte.
Il terreno e' reale. Le coordinate sono reali. La geologia e' reale.

L'interfaccia e' persistente con il Marte reale.
Non stai creando un mondo fantasy. Stai creando una sovrascrittura umana del pianeta reale.

---

## 2. Dati Reali — Fonti

### Fonti Principali (NASA/ESA)
| Fonte | Risoluzione | Uso |
|-------|-------------|-----|
| MOLA (Mars Orbiter Laser Altimeter) | ~460m/pixel globale | Terreno globale LOD basso |
| HiRISE (High Resolution Imaging) | 25-50cm/pixel locale | Dettaglio zone esplorate |
| CTX (Context Camera) | ~6m/pixel | LOD medio per transizione |
| Mars Express (ESA) | Variabile | Dati supplementari |
| THEMIS | ~100m/pixel termico | Composizione suolo |

### Dati Disponibili
- Altimetria precisa (heightmap globale)
- Texture reali (colore, albedo)
- Morfologie autentiche (crateri, canyon, vulcani)
- Crateri mappati con coordinate
- Rilievi 3D ad alta risoluzione

---

## 3. Implementazione Tecnica del Terreno

### Streaming Multi-LOD
```
LOD 0: MOLA globale (~460m/px) — sempre caricato
LOD 1: CTX regionale (~6m/px) — caricato per zona attiva
LOD 2: HiRISE locale (~25cm/px) — solo zone esplorate dal giocatore
LOD 3: Procedurale — dettaglio sub-pixel generato da heightmap reale
```

### Strategia di Caricamento
- Marte globale in MOLA LOD basso (sempre presente)
- Caricamento dinamico HiRISE solo in zone esplorate
- Compressione procedurale guidata da heightmap reale
- Streaming selettivo basato sulla posizione del giocatore
- Cache locale per zone gia' visitate

### Dimensioni Dati Stimate
- MOLA globale: ~2 GB
- CTX copertura parziale: ~50 GB (selezionato)
- HiRISE tile on-demand: ~500 MB per tile
- Storage locale totale: ~10-20 GB (cache dinamica)

---

## 4. Toponomastica — Il Sistema di Nomenclatura

### Due Layer Sovrapposti

#### Layer 1: Nomi IAU Ufficiali
- International Astronomical Union
- Nomi ufficiali (Olympus Mons, Valles Marineris, Gale Crater...)
- Fissi, non modificabili
- Visibili come riferimento

#### Layer 2: Nomi Civili Emergenti
- Creati dai giocatori
- Basati sulla scoperta e l'esplorazione
- Dinamici, contestabili, vivi

### Analogo Terrestre
Come Google Maps vs nomi storici.
Come OpenStreetMap vs toponimi ufficiali.
Il Marte del gioco ha nomi ufficiali E nomi civili.

---

## 5. Meccanica di Nomenclatura

### Come si nomina un luogo

1. **Scoperta** — Il giocatore esplora una zona senza nome civile
2. **Scansione** — Scansiona la formazione geologica
3. **Documentazione** — L'IA registra dati geologici
4. **Proposta** — Il giocatore propone un nome
5. **Registrazione** — Il nome appare nel Layer Civile
6. **Uso** — Altri giocatori iniziano a usarlo

### Regole della Nomenclatura (da decidere)

**Opzione A: Permanenti e non modificabili**
- Chi scopre, nomina. Per sempre.
- Pro: valore massimo della scoperta
- Contro: nomi stupidi permanenti, first-mover advantage

**Opzione B: Votabili dalla comunita'**
- Chiunque puo' proporre, la comunita' vota
- Pro: qualita' dei nomi, democrazia
- Contro: lento, politicizzato

**Opzione C: Acquistabili**
- I nomi hanno un costo energetico
- Si possono comprare/vendere naming rights
- Pro: economia emergente, scarsita'
- Contro: oligarchie toponomastiche

**Opzione D: Legati alla scoperta fisica in VR**
- Solo chi esplora in prima persona puo' nominare
- Pro: merito reale, esperienza significativa
- Contro: limita la partecipazione

**Raccomandazione:** Combinazione D + B
- Scoperta fisica necessaria per proporre
- Comunita' locale puo' contestare dopo X tempo
- Costo energetico per la registrazione

---

## 6. Effetti sulla Gameplay

### Identita' Territoriale
"L'uomo difende cio' che nomina."
Dare un nome crea attaccamento. L'attaccamento crea conflitto.

### Dinamiche Emergenti
- **Rivendicazioni territoriali** — "Questa e' Canyon Pacetti"
- **Guerre per landmark** — chi controlla Olympus Mons?
- **Monumenti su punti reali** — costruzioni commemorative
- **Turismo virtuale** — pellegrinaggi su Valles Marineris
- **Guerre simboliche** — rinominare le scoperte del nemico
- **Cartografia** — mappe civili come bene di scambio

### Micro vs Macro
- **Macro:** Olympus Mons, Valles Marineris (nomi IAU noti)
- **Micro:** migliaia di colline, crateri, canaloni senza nome
- Il vero gioco toponomastico e' nelle micro-strutture

---

## 7. Landmark Principali di Marte

| Nome | Tipo | Coordinate | Note |
|------|------|-----------|------|
| Olympus Mons | Vulcano | 18.65N, 226.2E | Il piu' alto del sistema solare (21.9 km) |
| Valles Marineris | Canyon | 14S, 294E | 4000 km di lunghezza |
| Hellas Planitia | Bacino | 42.7S, 70E | Cratere da impatto piu' profondo |
| Gale Crater | Cratere | 5.4S, 137.8E | Sito di Curiosity |
| Jezero Crater | Cratere | 18.4N, 77.7E | Sito di Perseverance |
| Tharsis | Regione | 0N, 260E | Altopiano vulcanico |
| Elysium Mons | Vulcano | 25N, 147.2E | Secondo grande vulcano |
| Utopia Planitia | Pianura | 49.7N, 118E | Pianura settentrionale |

---

## 8. Valore Culturale

Stai permettendo all'umanita' di iniziare a raccontare Marte.
Non "terraforming" nel senso fisico.
Ma: **Human Cultural Layering of Mars**.

E' un concetto filosofico enorme.
E' la prima toponomastica digitale partecipativa di un altro pianeta.
