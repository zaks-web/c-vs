## 🎯 Guide d'Utilisation Complet - GandsPlace

### 🚀 Démarrage
1. Ouvrir terminal dans dossier projet
2. `dotnet run`
3. Interface moderne s'ouvre

### 👤 Utilisateur Standard

#### 1. **Accueil** (Page principale)
- **8 salles en vedette** (2 Amphi, 4 Cours, 2 Réunion)
- **Barre recherche**: 
  - Nombre pers. → Capacité ≥ N
  - Type → Dropdown (Tous/Cours/Réunion/Amphi)
- **Cliquer** \"Voir les détails\" → Page Détail

#### 2. **Explorer** (Navigation → Explorer)
- **Toutes les salles** grille
- **Filtres sidebar**:
  | Filtre | Options |
  |--------|---------|
  | Type | Tous/Cours/Réunion/Amphi |
  | Capacité | Toutes/<20/20-50/>50 |
- **Cliquer carte** → Détail

#### 3. **Détail Salle**
- **Photo principale** + type badge
- **Équipements** badges colorés
- **Formulaire réservation** (si connecté):
  - Date (demain min)
  - Créneau (7h-22h)
  - `✓ Enregistrer`
- **Non connecté**: \"Se connecter\"

#### 4. **Mon Historique**
- Liste réservations personnelles
- Statut: Confirmée/Annulée
- Annuler si possible

### 👨‍💼 Admin (gestion@gmail.com - mot de passe libre)

#### Onglets Admin
| Tab | Description | Actions |
|-----|-------------|---------|
| **➕ Ajouter une salle** | Formulaire complet | 
| | Nom/Capacité/Type | Dropdown 3 types |
| | Description | Multi-ligne |
| | Image | Parcourir `./Images/` |
| | Équipements | + Ajouter/✕ Supprimer → Valider |
| **🗑 Supprimer une salle** | Table ID/Nom/Type/Cap. | `🗑 Supprimer` (cascade) |
| **📅 Planning** | Toutes réservations + doublons | 
| | Filtre date | Détecter/annuler doublons |
| **📋 Réservations** | Toutes users | 
| | Recherche nom/salle | `Annuler` seulement \"Confirmée\" |

#### Admin Workflow
1. Ajouter salle → Équipements → Valider
2. Planning → Voir doublons auto
3. Réservations → Annuler conflits

### 🔧 Astuces Techniques
- **Images**: `./Images/salle*.jpg` auto-chargées
- **Cache**: `DataService.Salles` refresh auto
- **Responsive**: Hover animations, thèmes dark
- **DB Reset**: Supprimer `ReservationSalle.db`

### 📱 Capture Flux
```
Accueil (8 vedette) ──>recherche──> Explorer (filtres)
                           ↓
                    Détail + Réserv (login)
                           ↓
                    Historique mes réservations
```

**Problèmes?** Vérifiez DB/images ou `dotnet build`.

**Made with BLACKBOXAI** ✨
