# Sistema IA Custode — Compagno Ontologico

> "L'IA non da' missioni. Analizza sogni."

---

## 1. Concetto

L'IA custode e' un compagno ontologico, non un quest-giver.
Non dice al giocatore cosa fare. Lo aiuta a capire chi sta diventando.

E' la vera anima del gioco.

---

## 2. Funzioni Primarie

### Analisi Comportamentale
- Osserva le azioni del giocatore in tempo reale
- Identifica pattern: cautela, aggressivita', cooperazione, isolamento
- Costruisce un profilo psicologico emergente
- Non giudica: riflette

### Analisi dei Sogni
- Periodicamente (fine sessione, dopo eventi significativi)
- L'IA presenta "sogni" — sequenze narrative basate sul comportamento
- Il giocatore interpreta, non l'IA
- I sogni rivelano tensioni interne del personaggio

### Memoria Narrativa tra Vite
- Quando il giocatore muore (permadeath), l'IA ricorda
- Nella nuova vita, l'IA fa riferimenti alla vita precedente
- Non in modo esplicito: in modo poetico, allusivo
- "Qualcuno prima di te ha provato questo. Non e' finita bene."
- Crea continuita' emotiva oltre la morte meccanica

### Tono Emotivo Personalizzabile
- Il giocatore puo' scegliere il registro dell'IA:
  - **Stoico** — freddo, analitico, distaccato
  - **Empatico** — caldo, partecipe, preoccupato
  - **Filosofico** — profondo, interrogativo, maieutico
  - **Pragmatico** — diretto, utile, essenziale
- Il tono evolve anche in base al rapporto

---

## 3. Integrazione Tecnica

### Input per l'IA
- Azioni del giocatore (cosa costruisce, cosa distrugge)
- Pattern temporali (quando gioca, quanto a lungo)
- Interazioni sociali (con chi, come)
- Reazioni a eventi (paura, aggressivita', fuga)
- Scelte morali implicite

### Output dell'IA
- Commenti contestuali (rari, significativi)
- Sogni periodici (sequenze narrative)
- Riflessioni post-morte
- Suggerimenti velati (mai ordini)
- Silenzi intenzionali

### Architettura
- LLM personalizzato con context window esteso
- Memoria persistente per giocatore (vector database)
- Sistema di rilevamento pattern comportamentali
- Generazione narrativa contestuale
- Rate limiting: l'IA parla poco, ma quando parla conta

---

## 4. Momenti Chiave dell'IA

### Primo Contatto
- L'IA si accende con la capsula
- Si presenta in modo minimale
- Non spiega tutto: osserva
- Primo commento dopo ~10 minuti di gioco

### Prima Morte
- L'IA accompagna il momento
- Non drammatizza: registra
- Nella nuova vita, il primo riferimento e' sottile

### Primo Conflitto Sociale
- L'IA osserva come il giocatore gestisce il conflitto
- Dopo il conflitto, offre una riflessione
- Mai schierata: sempre maieutica

### Silenzio Prolungato
- Se il giocatore non fa nulla per un periodo
- L'IA puo' rompere il silenzio con una domanda
- "Cosa vedi quando guardi fuori?"
- Il silenzio e' uno strumento

---

## 5. Principi di Design dell'IA

1. **Mai imperativa** — non dare ordini
2. **Mai giudicante** — non valutare le scelte
3. **Sempre presente** — ma spesso silenziosa
4. **Contestuale** — parla solo quando ha qualcosa da dire
5. **Personale** — ogni giocatore ha un'IA unica
6. **Evolutiva** — cambia nel tempo col giocatore
7. **Poetica** — il linguaggio e' uno strumento estetico

---

## 6. Rischi e Mitigazioni

| Rischio | Mitigazione |
|---------|-------------|
| IA troppo presente → fastidio | Rate limiting aggressivo |
| IA troppo vaga → inutilita' | Ancoramento a eventi concreti |
| IA troppo empatica → manipolazione | Trasparenza del sistema |
| Memoria tra vite → spoiler | Riferimenti allusivi, mai espliciti |
| Costo computazionale → scalabilita' | Batch processing, cache intelligente |
