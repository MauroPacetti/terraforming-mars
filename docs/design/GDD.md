# Terraforming Mars — Game Design Document
## Ecosistema Civile Persistente su Marte

---

## 1. Vision

**Terraforming Mars** non e' un gioco. E' un ecosistema civile persistente costruito sulla superficie reale di Marte.

Il progetto fonde quattro dimensioni in un unico sistema:
- **A) Gioco Commerciale** — progressione, tensione, ricompense
- **B) Esperimento Sociale** — dinamiche emergenti senza regole preimpostate
- **C) Laboratorio Economico** — economia energetica reale e macroeconomia vivente
- **D) Simulazione di Civilta' Futura** — prototipo digitale di colonizzazione

**Concetto chiave:** Human Cultural Layering of Mars — l'umanita' inizia a raccontare Marte.

---

## 2. Concept Fondamentale

### La Capsula Panopticon
Il giocatore opera dall'interno di una capsula di controllo remoto. Non cammina su Marte: lo gestisce, lo costruisce, lo plasma.

**Vantaggi di design:**
- Elimina la locomotion sickness in VR
- Intensifica l'esperienza psicologica (fragilita', isolamento)
- Trasforma il mondo esterno in "progetto", non in passeggiata
- Crea tensione costante: sei fragile, il mondo e' ostile

### Marte Reale come Base
Il terreno di gioco e' la superficie reale di Marte, basata su dati NASA:
- MOLA (topografia globale) per il LOD basso
- HiRISE (altissima risoluzione) per zone esplorate
- Caricamento dinamico e streaming selettivo
- Compressione procedurale guidata da heightmap reale

---

## 3. Pilastri di Design

### Pilastro 1: Isolamento + Controllo Remoto
Il giocatore non e' un eroe. E' un operatore solitario in una capsula.

### Pilastro 2: Assenza di Regole Sociali Preimpostate
Non ci sono leggi, fazioni, o strutture sociali predefinite.
Tutto emerge dal comportamento dei giocatori:
- Cartelli, monopoli, alleanze
- Guerre preventive, religioni
- Formazione di stati, oligarchie
- Distribuzione della ricchezza

### Pilastro 3: Permadeath Persistente
La morte e' permanente. Il mondo continua.
- Esperienza lenta, morte rara ma significativa
- La ricostruzione e' accelerata dalla conoscenza
- L'IA custode preserva la memoria narrativa tra vite

### Pilastro 4: Energia come Valuta Universale
Tutto costa energia. Energia = tempo = vita.
- Produzione, accumulo, trasporto, efficienza
- Inflazione naturale, crisi energetiche
- Collasso di infrastrutture, salvataggi cooperativi

### Pilastro 5: Toponomastica Civile Emergente
I giocatori scoprono, esplorano, e nominano il territorio.
- Layer Civile Virtuale sovrapposto ai nomi IAU ufficiali
- Identita' territoriale: l'uomo difende cio' che nomina
- Rivendicazioni, guerre per landmark, monumenti

---

## 4. Target e Modalita'

### Founders Hardcore
- Permadeath totale
- Nessuna protezione
- Economia pura
- Per i pionieri del sistema

### Civilian Safe Zone
- Rischio ridotto
- Accesso graduale al pericolo
- Progressione guidata
- Per allargare il pubblico

---

## 5. Unique Selling Points

1. **Primo ecosistema civile persistente su un pianeta reale**
2. **IA compagno ontologico** (non quest-giver, ma analista dei sogni)
3. **Economia energetica pura** come simulatore macroeconomico vivente
4. **Toponomastica partecipativa** — la prima nomenclatura civile di Marte
5. **Permadeath significativo** con memoria narrativa tra vite
6. **Server unico persistente** — un solo Marte condiviso
7. **Dati NASA reali** come fondamento del mondo

---

## 6. Documenti Collegati

- [Core Loop](./core-loop.md) — Le prime 3 ore e il ciclo di gioco
- [Economy & Energy](../systems/economy-energy.md) — Sistema economico ed energetico
- [Social Systems](../systems/social-governance.md) — Governance e dinamiche sociali
- [AI Companion](../systems/ai-companion.md) — Sistema IA custode
- [Mars Toponomastics](../systems/mars-toponomastics.md) — Dati reali e nomenclatura
- [Technical Architecture](../technical/architecture.md) — Architettura tecnica
- [Investor Pitch](../pitch/pitch-deck.md) — Documento per investitori
