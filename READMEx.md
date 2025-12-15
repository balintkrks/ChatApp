# ChatApp - TCP Alapú Kliens-Szerver Csevegőalkalmazás

Ez a projekt egy többfelhasználós, TCP/IP alapú csevegőalkalmazás (Chat Application), amely .NET környezetben, C# nyelven készült. [cite_start]A rendszer háromrétegű architektúrára épül, grafikus felhasználói felülettel (GUI), adatbázis-integrációval, fájlküldési lehetőséggel és mesterséges intelligencia (AI) támogatással rendelkezik[cite: 32, 35, 82].

##  Projekt Áttekintés

A projekt célja egy stabil kommunikációs rendszer létrehozása, amely lehetővé teszi a valós idejű üzenetváltást, a felhasználók kezelését és az adatok biztonságos tárolását.

### [cite_start]Készítők [cite: 173]
* **Kerekes Bálint**
* **Csongrádi Tibor**
* **Nagy Noel**

---

##  Funkciók

[cite_start]A rendszer az alábbi fő funkciókat tartalmazza a funkcionális specifikáció és a rendszerterv alapján[cite: 115, 125]:

### 1. Felhasználókezelés
* [cite_start]**Regisztráció:** Új felhasználók létrehozása, jelszavak biztonságos (SHA-256 hash) tárolása[cite: 71, 126].
* **Bejelentkezés:** Hitelesítés adatbázis alapján. [cite_start]Sikeres belépéskor a korábbi üzenetek betöltése[cite: 130, 264].
* **Anonim mód:** Lehetőség vendégként (Anon) való csatlakozásra regisztráció nélkül (kódbázis alapján).

### 2. Kommunikáció
* [cite_start]**Broadcast (Csoportos) chat:** Üzenet küldése minden csatlakozott kliensnek[cite: 140].
* [cite_start]**Privát üzenet:** 1:1 kommunikáció kiválasztott felhasználóval, amely mások számára nem látható[cite: 142].
* **Előzmények:** Az utolsó üzenetek (alapértelmezetten 20 db) betöltése belépéskor az SQLite adatbázisból.

### 3. Fájlküldés
* Fájlok átvitele byte-stream segítségével a szerveren keresztül.
* [cite_start]Támogatás mind a publikus, mind a privát fájlküldéshez[cite: 145, 93].

### [cite_start]4. Mesterséges Intelligencia (AI) Modul [cite: 148]
* **Chatbot:** A felhasználók beszélgethetnek egy AI asszisztenssel.
* **Moderáció:** Az AI figyeli a trágár kifejezéseket.
    * 1-2. alkalom: Figyelmeztetés.
    * 3. alkalom: A felhasználó automatikus kirúgása (Kick) a szerverről.

---

##  Technológiák és Architektúra

[cite_start]A rendszer **Kliens - Szerver - Adatbázis** felépítésű[cite: 35].

* **Programozási nyelv:** C# (.NET 8.0)
* [cite_start]**Környezet:** Windows 10/11, Visual Studio 2022 [cite: 175, 288]
* [cite_start]**Adatbázis:** SQLite (`Microsoft.Data.Sqlite`) - A `Users` és `Messages` táblák kezelésére[cite: 119, 336].
* [cite_start]**Hálózati kommunikáció:** TCP Sockets (`System.Net.Sockets`)[cite: 116].
* [cite_start]**Felhasználói felület (GUI):** Windows Forms (WinForms)[cite: 36].
* **AI Integráció:** OpenAI API (REST hívásokon keresztül).

### [cite_start]Modul Felépítés[cite: 39]:
* **ChatServer:** A központi szerver, kezeli a kapcsolatokat és az adatbázist.
* **ChatClientGUI:** A grafikus kliens alkalmazás.
* **ChatCommon:** Közös osztályok (Protokoll, Modellek) és segédfüggvények.
* **ChatAI:** Különálló kliensként futó bot szolgáltatás.

---

##  Telepítés és Futtatás

### Előfeltételek
* .NET 8.0 SDK telepítése.
* Visual Studio 2022 (ajánlott).

### Lépések

1. **Klónozás és Fordítás:**
   Töltsd le a forráskódot, nyisd meg a `ChatApp.sln` fájlt Visual Studio-ban, és fordítsd le a megoldást (Build Solution).

2. **Szerver Indítása:**
   Indítsd el a `ChatServer` projektet.
   * A szerver alapértelmezetten az `5000`-es porton figyel.
   * Első indításkor automatikusan létrehozza a `chat.db` adatbázisfájlt.

3. **AI Modul Indítása (Opcionális):**
   Ha szeretnéd használni a ChatBotot és a moderációt:
   * Nyisd meg a `ChatAI/AiService.cs` fájlt.
   * Cseréld le a `_apiKey` változó értékét egy érvényes OpenAI API kulcsra.
   * Indítsd el a `ChatAI` konzolalkalmazást.

4. **Kliens Indítása:**
   Indítsd el a `ChatClientGUI` alkalmazást (akár több példányban is).
   * **Szerver IP:** Alapértelmezetten `127.0.0.1:5000`.
   * **Login:** Használd a regisztrációt, vagy lépj be meglévő fiókkal.

---

##  Használati Útmutató

### [cite_start]Bejelentkezés [cite: 130]
* Indítás után add meg a felhasználóneved és jelszavad.
* Ha nincs fiókod, kattints a "Register" gombra.
* Ha csak nézelődni szeretnél, hagyj üresen mindent a "Login" gomb megnyomásakor (Anonim belépés).

### Üzenetküldés
* Írd be az üzenetet az alsó sávba, majd nyomj Entert vagy kattints a kék nyílra.
* [cite_start]**Privát üzenet:** Kattints duplán egy felhasználó nevére a jobb oldali listában a privát ablak megnyitásához[cite: 142].

### Fájlküldés
* Kattints a gémkapocs (📎) ikonra, válaszd ki a fájlt.
* [cite_start]A fogadó félnek rá kell kattintania az üzenetre ("CLICK TO SAVE") a mentéshez[cite: 147].

---

##  Ismert Hibák és Állapot (Kanban alapján)

[cite_start]A fejlesztés a 2025.12.06-os állapot szerint az alábbi javításokat tartalmazza[cite: 180]:

*  **Javítva:** Több kliens esetén fagyás (B00).
*  **Javítva:** Broadcast üzenetek kézbesítése (B01).
*  **Javítva:** Fájlküldés során sérült fájlok (B04).
*  **Javítva:** Privát üzenet biztonsága (B05).
*  **Javítva:** Az AI chatbot válaszadási stabilitása és moderációs funkciói még finomhangolás alatt állnak (B07, B08).

---

##  Biztonság

* [cite_start]A jelszavak nem plain text formátumban, hanem hash-elve tárolódnak[cite: 71].
* [cite_start]A rendszer képes kiszűrni és szankcionálni a nem megfelelő viselkedést az AI modul segítségével[cite: 74].
