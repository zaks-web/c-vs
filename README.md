# 🏛️ GANDS PLACE — Version Modernisée

Application de réservation de salles à Lomé, Togo — **WPF / C# / .NET 8**

---

## ✨ Nouveautés de cette version

| Fonctionnalité | Détail |
|---|---|
| 🎨 **Thème bleu nuit** | Palette complète `#0F1117` → accent `#4F8EF7` |
| ✨ **Animations fade** | Transition douce entre chaque page |
| 🖱️ **Hover scale** | Les cartes de salles s'agrandissent au survol |
| 💡 **Glow effects** | Boutons avec DropShadow coloré |
| 🗄️ **BDD auto-init** | Création + données au premier lancement |
| 🔍 **Filtres sidebar** | Explorer avec filtres type/capacité en temps réel |
| 📋 **Layout 2 colonnes** | Détail salle : infos + formulaire côte à côte |
| 🔒 **Auth dynamique** | Navbar s'adapte selon connexion/déconnexion |
| 📱 **Scrollbar custom** | Fine et discrète, cohérente avec le thème |

---

## 🚀 Lancer l'application

```bash
cd GandsPlace-modern
dotnet restore
dotnet run
```

**Compte par défaut :** `admin@gandsplace.com` / `admin123`

---

## 🗂️ Structure

```
GandsPlace-modern/
├── GantsPlace.csproj
├── App.xaml / App.xaml.cs          ← Init BDD au démarrage
├── MainWindow.xaml / .cs           ← Navbar + navigation animée
├── Models/Models.cs                ← Salle, Reservation, Utilisateur, Session
├── Services/
│   ├── DataService.cs              ← SQLite + seed automatique
│   └── ImageHelper.cs              ← Chargement images sécurisé
├── Styles/AppStyles.xaml           ← Thème complet (couleurs, boutons, inputs)
└── Views/
    ├── DemarragePage               ← Accueil équipe + stats
    ├── AccueilPage                 ← Grille 12 salles + recherche
    ├── ExplorerPage                ← 20 salles + filtres sidebar
    ├── DetailSallePage             ← Fiche salle + formulaire réservation
    ├── HistoriquePage              ← Réservations + annulation
    ├── LoginPage                   ← Connexion moderne
    ├── InscriptionPage             ← Inscription avec validation
    └── ContactPage                 ← Infos + formulaire message
```
