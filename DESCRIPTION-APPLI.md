# 📋 Description Détaillée - GandsPlace

## 🎯 Aperçu Général
**GandsPlace** est une **application WPF (.NET 8)** de réservation de salles ultra-moderne avec interface dark theme, animations fluides et gestion admin complète.

### Objectif
Permettre aux **étudiants/utilisateurs** de réserver facilement des salles + **admins** de gérer l'inventaire/planning.

## 🏗️ Architecture Technique

### Stack
- **Framework**: WPF (.NET 8)
- **DB**: SQLite (`ReservationSalle.db`)
- **Images**: Local `./Images/` (JPG/PNG auto)
- **Libs**: Microsoft.Data.Sqlite

### Structure Fichiers
```
GantsPlace-modern/
├── GantsPlace.csproj         # Projet principal
├── App.xaml                  # Styles globaux
├── MainWindow.xaml.cs        # Navigation + Auth
├── Models/Models.cs          # Salle {Id, Nom, TypeSalle, Capacite, Description, ImagePath, Equipements[]}
├── Services/
│   ├── DataService.cs        # CRUD (LoadSalles, AjouterReservation...)
│   └── ImageHelper.cs        # LoadImageBrush(path)
├── Views/                    # 10+ pages XAML
│   ├── AccueilPage.xaml      # 8 salles vedette (2/4/2)
│   ├── ExplorerPage.xaml     # Grille + sidebar filtres
│   ├── DetailSallePage.xaml  # Photo + form réservation
│   ├── AdminPage.xaml        # 4 tabs (Ajouter/Supprimer/Planning/Réservations)
│   ├── Login/Inscription     # Auth système
│   └── HistoriquePage.xaml   # Mes réservations
├── Images/                   # 20+ salle1.jpg → salle20.jpg
├── Styles/AppStyles.xaml     # Thème dark + animations
└── Docs/
    ├── README.md
    ├── GUIDE-UTILISATION.md
    └── DESCRIPTION-APPLI.md ← Vous
```

## 🛠️ Modèles de Données

### Classe Salle
```csharp
public class Salle 
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string TypeSalle { get; set; } // "Amphithéâtre"/"Salle de cours"/"Salle de réunion"
    public int Capacite { get; set; }
    public string Description { get; set; }
    public List<string> Equipements { get; set; } = new();
    public string? ImagePath { get; set; } // "salle1" → Images/salle1.jpg
}
```

### DB Tables
| Table | Colonnes |
|-------|----------|
| Salle | id_salle, nom_salle, type_salle, capacite_salle, description, image |
| Equipement | id_salle, nom_equipement |
| User | id_user, nom_user, email, motdepasse |
| Creneau | id_creneau, heure_debut, heure_fin |
| Reservation | id_reservation, id_salle, id_user, id_creneau, jour, statut, nom_salle |

## ✨ Fonctionnalités Page par Page

### 1. **AccueilPage** (`Views/AccueilPage.xaml`)
- **8 salles featured**: 2 Amphi + 4 Cours + 2 Réunion (random order)
- **Recherche**: Capacité + Type dropdown
- **Cards**: Photo/équipements badges/hover animation

### 2. **ExplorerPage** (`Views/ExplorerPage.xaml`)
- **Sidebar filtres**: Type (4), Capacité (4 options)
- **Grille responsive**: 260px cards
- **Badge count**: \"X salles\"

### 3. **DetailSallePage** (`Views/DetailSallePage.xaml`)
- **Photo 360px** (overlay icon si pas image)
- **Équipements** badges colorés
- **Form réservation**: DatePicker + ComboBox créneaux
- **Login prompt** si non connecté

### 4. **AdminPage** (`Views/AdminPage.xaml`)
**4 onglets tabs**:
1. **➕ Ajouter**: Form + image picker + multi-équipements
2. **🗑 Supprimer**: Table filtrable → cascade delete
3. **📅 Planning**: Doublons auto-détectés + filtre date
4. **📋 Réservations**: Toutes users, **Action vide si "Annulée"**

### 5. **Auth System**
- **LoginPage/InscriptionPage**: Email/mdp
- **Admin only**: gestion@gmail.com
- **Session**: Static `Session.EstConnecte` + `UtilisateurConnecte`

## 🎨 UI/UX Détails
- **Dark Theme**: BgDeepBrush (#0D1117), cards #1C2030
- **Animations**: Scale hover (1.02x), fade navigation
- **Badges**: Type/capacité color-coded
- **Responsive**: ScrollViewer + WrapPanel

## 🚀 Services Clés
- **DataService.LoadSalles()**: Cache global + ImagePath depuis DB
- **ImageHelper.LoadImageBrush()**: `./Images/{path}.jpg/png`
- **DataService.AjouterReservation()**: Insert + nom_salle auto

## 📊 Exemple Données
```
DB> SELECT * FROM Salle LIMIT 5;
1|Amphi A|Amphithéâtre|150|Grand amphi|salle1
...

Accueil: [Amphi A, Amphi B, Cours1-4, Réunion1-2]
```

**Application Production-Ready** - Tous bugs fixés par BLACKBOXAI.
