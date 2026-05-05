# Personal-Keyboard-Mapper 

## 📌 Opis projektu

"Personal-Keyboard-Mapper" to aplikacja desktopowa dla systemu Windows, umożliwiająca pełną obsługę komputera przy użyciu ograniczonej liczby klawiszy – w szczególności klawiatury numerycznej.

Projekt powstał jako rozwiązanie wspierające osoby z ograniczeniami ruchowymi, które nie są w stanie korzystać z tradycyjnych urządzeń wejściowych (klawiatury i myszy) w standardowy sposób. Aplikacja pozwala na zastąpienie pełnej klawiatury zestawem prostych, sekwencyjnych kombinacji dwuklawiszowych.

---

## 🎯 Cel projektu

Celem projektu jest:

* umożliwienie efektywnej pracy przy komputerze osobom z niepełnosprawnościami ruchowymi,
* minimalizacja wymagań dotyczących precyzji ruchów użytkownika,
* zapewnienie pełnej funkcjonalności standardowej klawiatury i myszy przy użyciu niewielkiego urządzenia wejściowego (np. klawiatury numerycznej),
* zwiększenie dostępności technologii komputerowej.

---

## ⚙️ Główne funkcjonalności

### 🔢 Mapowanie klawiszy

* Każdy znak lub akcja przypisywana jest do **kombinacji dwóch klawiszy numerycznych**.
* Klawisze naciskane są sekwencyjnie (nie jednocześnie).

### ⌨️ Obsługa klawiatury

* Symulacja wszystkich klawiszy alfanumerycznych.
* Obsługa klawiszy funkcyjnych: `Shift`, `Ctrl`, `Alt`, `Win`.
* Możliwość użycia klawiszy funkcyjnych:

  * jednorazowo (dla jednego znaku),
  * w trybie ciągłym (np. pisanie wielkimi literami).

### 🖱️ Obsługa myszy

* Symulacja:

  * kliknięcia lewego/prawego przycisku,
  * podwójnego kliknięcia,
  * przytrzymania przycisku myszy.

### 📝 Wprowadzanie tekstu

* Możliwość przypisania całych ciągów znaków do jednej kombinacji.

### ⚙️ Konfiguracja

* Tworzenie i edycja konfiguracji w dedykowanym edytorze.
* Zapisywanie konfiguracji do plików (`.keysconfig`, format JSON).
* Obsługa wielu konfiguracji i przełączanie między nimi.

### 🔊 Opcje pomocnicze

* Dźwięki informujące o akcjach (różne dla typów operacji).
* Okno podpowiedzi dla rozpoczętych kombinacji.

### ⏯️ Kontrola działania

* Możliwość wstrzymania i wznowienia działania programu (przywrócenie standardowej funkcji klawiatury numerycznej).

---

## 💻 Wymagania systemowe

* System operacyjny: Windows (Vista / 7 / 8 / 10)
* .NET Framework: wersja 4.5.2 lub nowsza
* Urządzenie wejściowe:

  * klawiatura numeryczna (wbudowana lub zewnętrzna)

---

## 🚀 Instalacja

1. Pobierz paczkę z programem.
2. Rozpakuj pliki do wybranego folderu.
3. Upewnij się, że pliki:

   * `Personal-Keyboard-Mapper.exe`
   * `WindowsInput.dll`
     znajdują się w tym samym katalogu.
4. Uruchom plik:

   ```
   Personal-Keyboard-Mapper.exe
   ```

---

## 🧩 Pierwsze uruchomienie

* Po uruchomieniu program wyświetli pustą tablicę przypisań.
* Aby rozpocząć:

  1. Kliknij **„Nowa konfiguracja”**.
  2. Zdefiniuj przypisania klawiszy.
  3. Zapisz konfigurację.

---

## 🛠️ Przykładowe zastosowanie

Program sprawdza się szczególnie w sytuacjach:

* sterowania komputerem przy użyciu minimalnego zakresu ruchów (np. głowy),
* pracy z alternatywnymi urządzeniami wejściowymi,
* budowy niestandardowych stanowisk komputerowych dla osób z niepełnosprawnościami.

---

## 📬 Kontakt

W razie pytań lub sugestii:
**[krulon@o2.pl](mailto:krulon@o2.pl)**

---

## 📄 Licencja

*(Uzupełnij zgodnie z wybraną licencją projektu, np. MIT / GPL)*

---
