# Contexto del proyecto - Umbral

## Minijuego de Puertas - Cambios realizados (13/06/2026)

### Resumen
Se modificó la lógica de la **puerta correcta** del minijuego de puertas (`DoorSelector.cs`) para que tenga una secuencia de **dos fases** (entrada y salida del consultorio) con un delay de por medio simulando la consulta médica.

### Archivos modificados

1. **`Assets/Scripts/Dialogue/NPC_Doctor_01.asset`**
   - Entrada de diálogo "Buen día, pase" cambió de `requiredProgress: 2` → `1`

2. **`Assets/Scripts/Dialogue/DialogueManager.cs`**
   - `PlayDialogueDirect` ahora acepta `int requiredProgress = -1` (opcional)
   - Si `requiredProgress >= 0`, filtra solo las entradas con ese valor
   - Si no hay entradas coincidentes, invoca el callback inmediatamente

3. **`Assets/Scripts/Minigames/DoorSelector.cs`**
   - Nuevo campo `consultationDelay` (float, default 3f)
   - Nueva secuencia para `isCorrect`:
     - `ShowDoctorAndPlayDialogue(1)` — sprite aparece + diálogo entrada (progress 1)
     - `HideDoctor()` — sprite desaparece (personaje entró al consultorio)
     - `WaitForSeconds(consultationDelay)` — pausa (consulta médica)
     - `questProgress = 3`
     - `ShowDoctorAndPlayDialogue(3)` — sprite aparece + diálogo salida (progress 3)
     - `HideDoctor()` — sprite desaparece
     - Reactivación del trigger
   - Métodos helper: `ShowDoctorAndPlayDialogue(int requiredProgress)` y `HideDoctor()`

### Flujo de questProgress
- `0` → Usar terminal (sacar ticket)
- `1` → Elegir puerta correcta (diálogo "Buen día, pase")
- `3` → Post-consulta (diálogo del electro)
