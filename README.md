# LOGICIEL BDD MUSEUM NIMES

Projet WPF développé pour le **Muséum d'Histoire Naturelle de Nîmes** en collaboration avec **Mathis Mourrau Farot**.

## 🎯 Objectif

Ce logiciel a été conçu pour répondre à deux besoins principaux du muséum :

1. **Gestion interne** : Permet au personnel d'ajouter, modifier ou supprimer des espèces dans une base de données centralisée.
2. **Consultation publique** : Offre aux visiteurs une interface intuitive pour explorer les différentes espèces exposées et accéder à leurs fiches d'informations.

## 🛠️ Technologies utilisées

- **Langage :** C#  
- **Framework :** .NET WPF (Windows Presentation Foundation)  
- **Base de données :** SQL Server (ou autre, selon configuration locale)  
- **ORM :** Entity Framework (si utilisé)

## 🧭 Fonctionnalités

### Pour les administrateurs (personnel du musée)
- Authentification sécurisée
- Ajout d'une nouvelle espèce avec :
  - Nom scientifique
  - Nom commun
  - Description
  - Image
  - Catégorie / Famille
- Modification et suppression d’espèces
- Interface de gestion simple et ergonomique

### Pour les visiteurs
- Navigation dans le catalogue des espèces
- Recherche par nom ou catégorie
- Affichage de fiches détaillées avec :
  - Images
  - Informations biologiques
  - Habitat, régime alimentaire, etc.

## 📦 Installation

1. Cloner le dépôt :
   ```bash
   git clone https://github.com/AzmogEx/LOGICIEL-BDD-MUSEUM-NIMES.git
   ```
   
Ouvrir la solution dans Visual Studio.

Configurer la chaîne de connexion à votre base de données dans le fichier App.config ou appsettings.json.

Compiler et exécuter le projet.

👥 Auteurs
Adam Marzuk
Mathis Mourrot Farot

🏛️ Remerciements
Merci au Muséum d'Histoire Naturelle de Nîmes pour leur collaboration et leur accueil dans le cadre de ce projet.

